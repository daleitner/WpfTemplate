using System;
using SC.Base.WPF.Commands;

namespace SC.Test.Common.Extensions
{
	public static class RelayCommandExtension
	{
		public static void ExecuteForTest(this RelayCommand command, object parameter)
		{
			command.Execute(parameter);
			if(command.IsError())
				throw new Exception("Command Failed", command.GetError());
		}
	}
}
