using System;

namespace BilibiliApi
{
	public static class Timestamp
	{
		public static DateTime GetTime(string timeStamp)
		{
			var dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
			var lTime = long.Parse($@"{timeStamp}0000000");
			var toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}

		public static string GetTimestamp()
		{
			return $@"{(long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}";
		}
	}
}
