using System.Text.Json;

namespace BilibiliMangaAutoClockIn.Model
{
	public class ClockInResponse
	{
		public int Code = -1;
		public string Message = @"";

		public ClockInResponse(string json)
		{
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;
			if (root.TryGetProperty(@"code", out var codeProperty) && codeProperty.ValueKind == JsonValueKind.Number)
			{
				codeProperty.TryGetInt32(out Code);
			}

			if (root.TryGetProperty(@"msg", out var msg))
			{
				Message = msg.GetString();
			}
		}
	}
}
