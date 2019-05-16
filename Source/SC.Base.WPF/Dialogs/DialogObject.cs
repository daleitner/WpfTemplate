using System;
using SC.Base.WPF.ViewModels;

namespace SC.Base.WPF.Dialogs
{
	public class DialogObject
	{
		public DialogObject()
		{
			DialogSize = DialogSizeEnum.Default;
			DialogButtons = DialogButtonsEnum.None;
		}
		public WorkspaceViewModel Content { get; set; }
		public string Message { get; set; }
		public string Caption { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public DialogSizeEnum DialogSize { get; set; }
		public DialogButtonsEnum DialogButtons { get; set; }
		public bool IsModal { get; set; }
		public bool IsResizeable { get; set; }
		public EventHandler ClosedEventHandler { get; set; }
	}
}
