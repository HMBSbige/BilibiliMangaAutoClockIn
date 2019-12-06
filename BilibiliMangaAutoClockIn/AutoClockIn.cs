using BilibiliApi;
using BilibiliMangaAutoClockIn.Model;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BilibiliMangaAutoClockIn
{
	public class AutoClockIn
	{
		private string UserName;
		private string Password;
		private readonly BilibiliToken _token = new BilibiliToken();

		public AutoClockIn(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		public AutoClockIn(string userName, string password, string accessToken, string refreshToken) : this(userName, password)
		{
			_token.AccessToken = accessToken;
			_token.RefreshToken = refreshToken;
		}

		private async Task<bool> GetClockInfo()
		{
			try
			{
				//查询签到
				var info = new ClockInInfo(await Manga.GetInfo(_token.AccessToken));
				if (info.Code == 0)
				{
					if (info.Status)
					{
						Console.WriteLine($@"已签到，已连续签到 {info.DayCount} 天");
						return true;
					}
					Console.WriteLine($@"未签到，已连续签到 {info.DayCount} 天");
				}
				else
				{
					Console.WriteLine($@"查询签到失败：{info.Message}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return false;
		}

		private async Task<bool> ClockIn()
		{
			try
			{
				if (await GetClockInfo())
				{
					return true;
				}

				var res = new ClockInResponse(await Manga.ClockIn(_token.AccessToken));
				if (res.Code == 0)
				{
					Console.WriteLine(@"签到成功");
					return true;
				}

				if (res.Message == @"clockin clockin is duplicate")
				{
					Console.WriteLine(@"不能重复签到");
					return true;
				}

				Console.WriteLine(res.Message);
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			Console.WriteLine(@"签到失败");
			return false;
		}

		private async Task<bool> LogIn()
		{
			try
			{
				var logKey = new LogInKey(await Passport.GetHash());
				if (logKey.Code == 0)
				{
					_token.Parse(await Passport.Login(logKey.Hash, logKey.Key, UserName, Password));
					if (_token.Code == 0)
					{
						Console.WriteLine(@"登录成功");
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.WriteLine(@"登录失败");
			return false;
		}

		private async Task<bool> ClockInTask()
		{
			if (!string.IsNullOrWhiteSpace(_token.AccessToken))
			{
				if (await ClockIn())
				{
					return true;
				}
			}
			return await LogIn() && await ClockIn();
		}

		private async Task<DateTime> ExpireTime()
		{
			try
			{
				var tokenInfo = new BilibiliToken(await Passport.GetTokenInfo(_token.AccessToken));
				return tokenInfo.Expires;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return DateTime.UtcNow;
		}

		private async Task<bool> RefreshToken()
		{
			try
			{
				var json = await Passport.RefreshToken(_token.AccessToken, _token.RefreshToken);
				var tokenInfo = new BilibiliToken(json);
				if (tokenInfo.Code == 0)
				{
					_token.Parse(json);
					Console.WriteLine(@"Token 刷新成功");
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			Console.WriteLine(@"Token 刷新失败");
			return false;
		}

		public async Task StartAsync(int retryTime)
		{
			++retryTime;
			while (true)
			{
				var i = 0;
				for (; i < retryTime; ++i)
				{
					if (await ClockInTask())
					{
						break;
					}
					if (i + 1 == retryTime)
					{
						++i;
						break;
					}
					var delay = TimeSpan.FromSeconds(10);
					Console.WriteLine($@"等待 {delay.TotalSeconds} 秒后重试：{i + 1}/{retryTime}");
					await Task.Delay(delay);
				}
				if (i < retryTime)
				{
					var time = await ExpireTime();
					if (time <= DateTime.UtcNow)
					{
						Console.WriteLine($@"Token 过期（{time.AddHours(8)}），重新登录");
						await LogIn();
					}
					else if (time - DateTime.UtcNow < TimeSpan.FromDays(7))
					{
						Console.WriteLine($@"Token 即将过期（{time.AddHours(8)}），尝试刷新 Token");
						if (await RefreshToken())
						{
							_token.Expires = await ExpireTime();
							Console.WriteLine($@"Token 有效期至 {_token.Expires.AddHours(8)}");
						}
					}
				}
				var waitTime = TimeSpan.FromMilliseconds(await GetCountdown()).Add(TimeSpan.FromSeconds(1));
				Console.WriteLine($@"等待 {waitTime:hh\:mm\:ss} 后执行");
				await Task.Delay(waitTime);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		private static async Task<double> GetCountdown()
		{
			var now = await Utils.GetCurrentTime();
			now = now.AddHours(8);
			Console.WriteLine($@"现在时间：{now.ToString(CultureInfo.CurrentCulture)}");

			var nextDay = now.Date.AddDays(1);
			Console.WriteLine($@"下次签到时间：{nextDay.ToString(CultureInfo.CurrentCulture)}");

			return (nextDay - now).TotalMilliseconds;
		}
	}
}
