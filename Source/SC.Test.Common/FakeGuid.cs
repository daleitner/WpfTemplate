using System;

namespace SC.Test.Common
{
	public static class FakeGuid
	{
		private static int _privateGuidNumber;

		public static Func<Guid> GetNewGuidFunction(int startIndex = 0)
		{
			ResetGuid(startIndex);
			return NewGuid;
		}

		public static Guid NewGuid()
		{
			return Int2Guid(_privateGuidNumber++);
		}

		private static Guid Int2Guid(int value)
		{
			byte[] bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		public static void ResetGuid(int startIndex = 0)
		{
			_privateGuidNumber = startIndex;
		}
	}
}
