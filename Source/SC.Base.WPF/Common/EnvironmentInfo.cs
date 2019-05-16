using System;

namespace SC.Base.WPF.Common
{
	public class EnvironmentInfo
	{
		private static EnvironmentInfo _environment;

		private EnvironmentInfo(string applicationName, string version)
		{
			ApplicationName = applicationName;
			Version = version;
		}

		public static EnvironmentInfo Instance {
			get
			{
				if(_environment == null)
					throw new NullReferenceException("You must call Create(applicationName, version) first!");
				return _environment;
			}
		}

		public static void Create(string applicationName, string version)
		{
			_environment = new EnvironmentInfo(applicationName, version);
		}

		public string ApplicationName { get; }

		public string Version { get; }
	}
}
