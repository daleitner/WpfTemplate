using System.Windows;
using log4net.Config;
using SC.Base.WPF.Common;

namespace %projectname%
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			// configure log4net and use it as soon as possible otherwise logging messages might get lost
			XmlConfigurator.Configure();
			EnvironmentInfo.Create("%projectname%", "1.0.0");
			base.OnStartup(e);
		}
	}
}
