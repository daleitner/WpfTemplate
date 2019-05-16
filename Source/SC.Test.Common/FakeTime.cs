using System;

namespace SC.Test.Common
{
	public static class FakeTime
	{
		private static int _incrementSecond;
		private static readonly DateTime DefaultDate = new DateTime(2017, 2, 2, 0, 0, 0);

		public static Func<DateTime> GetNewNow(int seconds = 0)
		{
			Reset();
			_incrementSecond = seconds;
			return Now;
		}

		private static DateTime Now()
		{
			return MakeDateTime(_incrementSecond);
		}

		private static DateTime MakeDateTime(int value)
		{
			return DefaultDate.AddSeconds(value);
		}

		private static void Reset()
		{
			_incrementSecond = 0;
		}
	}
}
