using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilibiliApi
{
	public class Manga : HttpRequest
	{
		private const string BaseUri = @"https://manga.bilibili.com";

		public static async Task<string> GetInfo(string accessKey)
		{
			const string requestUri = @"twirp/activity.v1.Activity/GetClockInInfo";
			var pair = new Dictionary<string, string>
			{
					{@"access_key", accessKey}
			};
			using var body = await GetBody(pair, false);
			return await PostAsync(BaseUri, requestUri, body);
		}

		public static async Task<string> ClockIn(string accessKey)
		{
			const string requestUri = @"twirp/activity.v1.Activity/ClockIn";
			var pair = new Dictionary<string, string>
			{
					{@"access_key", accessKey},
					{@"platform", @"android"}
			};
			using var body = await GetBody(pair, false);
			return await PostAsync(BaseUri, requestUri, body);
		}
	}
}
