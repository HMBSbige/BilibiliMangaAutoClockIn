using System.Text.Json;

namespace BilibiliMangaAutoClockIn.Model
{
	public class GetKey
	{
		public string Hash = @"";
		public string Key = @"";
		public int Code = -1;

		public GetKey(string json)
		{
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;
			if (root.TryGetProperty(@"code", out var codeProperty) && codeProperty.TryGetInt32(out Code) && Code == 0)
			{
				if (root.TryGetProperty(@"data", out var data))
				{
					if (data.TryGetProperty(@"hash", out var hash) && data.TryGetProperty(@"key", out var key))
					{
						Hash = hash.GetString();
						Key = key.GetString();
					}
				}
			}
		}
	}
}
