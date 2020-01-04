using BilibiliApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UnitTest
{
	[TestClass]
	public class UnitTest
	{
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
