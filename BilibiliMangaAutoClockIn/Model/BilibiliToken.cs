using BilibiliApi;
using System;
using System.Text.Json;

namespace BilibiliMangaAutoClockIn.Model
{
	public class BilibiliToken
	{
		public int Code = -1;
		public long Uid = 0;
		public string AccessToken = @"";
		public string RefreshToken = @"";

		/// <summary>
		/// UTC
		/// </summary>
		public DateTime Expires;

		public BilibiliToken(string json)
		{
			Parse(json);
		}

		public void Parse(string json)
		{
			Code = -1;
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;
			if (root.TryGetProperty(@"code", out var codeProperty) && codeProperty.TryGetInt32(out Code) && Code == 0)
			{
				if (root.TryGetProperty(@"ts", out var tsProperty))
				{
					var baseTime = Timestamp.GetTime(tsProperty.GetRawText());
					if (root.TryGetProperty(@"data", out var data))
					{
						if (data.TryGetProperty(@"mid", out var mid) && mid.TryGetInt64(out Uid)
						&& data.TryGetProperty(@"access_token", out var accessToken)
						&& data.TryGetProperty(@"expires_in", out var expiresInProperty)
						&& expiresInProperty.TryGetInt64(out var expiresIn))
						{
							AccessToken = accessToken.GetString();
							Expires = baseTime + TimeSpan.FromSeconds(expiresIn);
						}
						if (data.TryGetProperty(@"refresh_token", out var refreshToken))
						{
							RefreshToken = refreshToken.GetString();
						}
					}
				}
			}
		}

	}
}
