using System;

namespace SC.Base.WPF.SystemUtilities
{
	public static class SystemGuid
	{
		public static Func<Guid> NewGuid = Guid.NewGuid;

		public static void ResetNewGuid()
		{
			NewGuid = Guid.NewGuid;
		}
	}
}
