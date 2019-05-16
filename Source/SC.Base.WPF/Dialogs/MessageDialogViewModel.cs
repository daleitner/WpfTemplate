using System;
using SC.Base.WPF.ViewModels;

namespace SC.Base.WPF.Dialogs
{
	public class MessageDialogViewModel : WorkspaceViewModel
	{
		public MessageDialogViewModel(string message)
		{
			Message = message;
		}

		public string Message { get; set; }
	}
}
