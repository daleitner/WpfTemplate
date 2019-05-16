using System;

namespace SC.Base.WPF.SystemUtilities
{
	public static class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.Now;

		public static Func<DateTime> Today = () => DateTime.Today;
		public static void ResetNow()
		{
			Now = () => DateTime.Now;
		}

		public static void ResetToday()
		{
			Today = () => DateTime.Today;
		}
	}
}
