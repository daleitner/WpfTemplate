using System;
using NUnit.Framework;

namespace SC.Test.Common
{
	/// <summary>
	/// Set this attribute for failing tests which should be excluded from test execution at the build server.
	/// A reason in mandatory.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class ExcludeFromBuildServerAttribute : CategoryAttribute
	{
		public ExcludeFromBuildServerAttribute(string reason)
		{
			Reason = reason;
		}

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		private string Reason { get; }
	}
}