using System.Text.Json;

namespace BilibiliMangaAutoClockIn.Model
{
	public class ClockInInfo
	{
		public int Code = -1;
		public string Message = @"";
		public int DayCount = 0;
		public bool Status = false;

		public ClockInInfo(string json)
		{
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;
			if (root.TryGetProperty(@"code", out var codeProperty) && codeProperty.TryGetInt32(out Code) && Code == 0)
			{
				if (root.TryGetProperty(@"msg", out var msg))
				{
					Message = msg.GetString();
				}
				if (root.TryGetProperty(@"data", out var data)
				&& data.TryGetProperty(@"day_count", out var dc) && dc.TryGetInt32(out DayCount)
				&& data.TryGetProperty(@"status", out var status))
				{
					Status = status.GetRawText() == @"1";
				}
			}
		}
	}
}
