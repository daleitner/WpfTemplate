using System.Windows.Input;
using SC.Base.WPF.Commands;
using SC.Base.WPF.Common;
using SC.Base.WPF.ViewModels;

namespace %projectname%.Main
{
	public class MainViewModel : WorkspaceViewModel
	{
		public MainViewModel()
		{
		}

		public string Title => EnvironmentInfo.Instance.ApplicationName + " - Version " + EnvironmentInfo.Instance.Version;
	}
}
