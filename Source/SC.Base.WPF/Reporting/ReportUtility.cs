using System;
using log4net;

namespace SC.Base.WPF.Reporting
{
	public static class ReportUtility
	{
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static ExceptionDialogWindow _window;
		public static void Report(Exception exception)
		{
			Log.Error("Exception occured! " + exception);
			_window = new ExceptionDialogWindow();
			var viewModel = new ExceptionDialogViewModel(exception, _window);
			viewModel.RequestViewClose += ViewModel_RequestViewClose;
			_window.DataContext = viewModel;
			_window.ShowDialog();
		}

		private static void ViewModel_RequestViewClose(object sender, EventArgs e)
		{
			_window.Close();
		}
	}
}
