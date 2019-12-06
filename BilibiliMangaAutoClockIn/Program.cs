namespace BilibiliMangaAutoClockIn
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			AutoClockIn? task = null;
			if (args.Length == 2)
			{
				task = new AutoClockIn(args[0], args[1]);
			}
			else if (args.Length == 4)
			{
				task = new AutoClockIn(args[0], args[1], args[2], args[3]);
			}
			task?.StartAsync(5).Wait();
		}
	}
}
