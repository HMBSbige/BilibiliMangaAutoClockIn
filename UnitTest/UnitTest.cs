using BilibiliApi;
using BilibiliMangaAutoClockIn.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UnitTest
{
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public async Task TestLogin()
		{
			const string userName = @"";
			const string password = @"";
			var token = new BilibiliToken();
			var hash = await Passport.GetHash();
			var logKey = new LogInKey(hash);
			Console.WriteLine(hash);
			Assert.AreEqual(0, logKey.Code);
			var res = await Passport.Login(logKey.Hash, logKey.Key, userName, password);
			Console.WriteLine(res);
			token.Parse(res);
			Assert.AreEqual(0, token.Code);

		}

		[TestMethod]
		public async Task TestRefreshToken()
		{
			var accessToken = @"";
			var refreshToken = @"";
			var json = await Passport.RefreshToken(accessToken, refreshToken);
			Console.WriteLine(json);
		}

		[TestMethod]
		public async Task TestRevoke()
		{
			var accessToken = @"";
			var json = await Passport.Revoke(accessToken);
			Console.WriteLine(json);
		}
	}
}
