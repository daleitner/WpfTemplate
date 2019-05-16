using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.Base.WPF.Dialogs
{
	public class DialogUtility : IDialogUtility
	{
		private static DialogWindow _window;
		private static DialogUtility _instance;
		public static DialogUtility Instance => _instance ?? (_instance = new DialogUtility());

		public DialogResultEnum Show(DialogObject dialogObject)
		{
			var content = dialogObject.Content ?? new MessageDialogViewModel(dialogObject.Message);

			var dialogObj = new DialogObject
			{
				Content = content,
				Caption = dialogObject.Caption,
				DialogButtons = dialogObject.DialogButtons
			};

			var viewModel = new DialogViewModel(dialogObj);
			viewModel.RequestViewClose += ViewModel_RequestViewClose;
			double width;
			double height;
			switch (dialogObject.DialogSize)
			{
				case DialogSizeEnum.Small:
					width = 500;
					height = 300;
					break;
				case DialogSizeEnum.Medium:
					width = 900;
					height = 600;
					break;
				case DialogSizeEnum.Large:
					width = 1300;
					height = 900;
					break;
				case DialogSizeEnum.Maximum:
					width = System.Windows.SystemParameters.WorkArea.Width;
					height = System.Windows.SystemParameters.WorkArea.Height;
					break;
				default:
					width = dialogObject.Width;
					height = dialogObject.Height;
					break;
			}

			_window = new DialogWindow
			{
				DataContext = viewModel,
				Width = width,
				Height = height,
				MaxWidth = System.Windows.SystemParameters.WorkArea.Width,
				MaxHeight = System.Windows.SystemParameters.WorkArea.Height,
				ResizeMode = dialogObject.IsResizeable ? System.Windows.ResizeMode.CanResize : System.Windows.ResizeMode.NoResize
			};

			if (width < 0.1 && height < 0.1)
				_window.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;

			if (dialogObject.ClosedEventHandler != null)
				_window.Closed += dialogObject.ClosedEventHandler;

			if (dialogObject.IsModal)
				_window.ShowDialog();
			else
				_window.Show();
			return viewModel.DialogResult;
		}

		public DialogResultEnum Show(string message, string caption)
		{
			return Show(new DialogObject
			{
				Message = message,
				Caption = caption,
				IsModal = true,
				DialogButtons = DialogButtonsEnum.Ok,
				DialogSize = DialogSizeEnum.Small,
				IsResizeable = true
			});
		}

		private static void ViewModel_RequestViewClose(object sender, EventArgs e)
		{
			_window.Close();
		}
	}
}
