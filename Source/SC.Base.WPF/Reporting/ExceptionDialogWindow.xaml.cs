using System.Windows.Input;

namespace SC.Base.WPF.Reporting
{
	/// <summary>
	/// Interaction logic for ExceptionDialogWindow.xaml
	/// </summary>
	public partial class ExceptionDialogWindow
	{
		public ExceptionDialogWindow()
		{
			InitializeComponent();
		}

		private void labelHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
			e.Handled = true;
		}
	}
}
