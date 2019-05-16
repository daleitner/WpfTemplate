using ApprovalTests;
using ApprovalTests.Reporters;
using %projectname%.Main;
using NUnit.Framework;
using SC.Test.Common.Extensions;

namespace %projectname%_Unit
{
	[TestFixture]
	[UseReporter(typeof(WinMergeReporter))]
	public class MainViewModelTests
	{
		private MainViewModel SUT;

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void VerifyInitializedSUT()
		{
			InitializeSUT();
			Approvals.Verify(SUT.ToPrettyString());
		}

		private MainViewModel InitializeSUT()
		{
			SUT = new MainViewModel();
			return SUT;
		}
	}
}
