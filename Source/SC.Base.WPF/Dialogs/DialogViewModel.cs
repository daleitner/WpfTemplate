using System;
using System.Windows;
using System.Windows.Input;
using SC.Base.WPF.Commands;
using SC.Base.WPF.ViewModels;

namespace SC.Base.WPF.Dialogs
{
	public class DialogViewModel : WorkspaceViewModel
	{
		private WorkspaceViewModel _childViewModel;

		private RelayCommand _okCommand;
		private RelayCommand _cancelCommand;
		private RelayCommand _yesCommand;
		private RelayCommand _noCommand;

		public DialogViewModel(DialogObject dialogObject)
		{
			ChildViewModel = dialogObject.Content;
			HeaderText = dialogObject.Caption;
			DialogButtons = dialogObject.DialogButtons;
			DialogResult = DialogResultEnum.Undefined;
			SetButtonVisibilities();
		}

		public WorkspaceViewModel ChildViewModel
		{
			get
			{
				return _childViewModel;
			}
			set
			{
				_childViewModel = value;
				SubscribeToCloseEvent();
				OnPropertyChanged(nameof(ChildViewModel));
			}
		}

		public string HeaderText { get; set; }

		public DialogResultEnum DialogResult
		{
			get
			{
				if (_dialogResult == DialogResultEnum.Undefined)
					return DefaultDialogResult();
				return _dialogResult;
			}
			set => _dialogResult = value;
		}

		private DialogResultEnum DefaultDialogResult()
		{
			switch (DialogButtons)
			{
				case DialogButtonsEnum.Ok:
					return DialogResultEnum.Ok;
				case DialogButtonsEnum.YesNo:
					return DialogResultEnum.No;
				case DialogButtonsEnum.Cancel:
				case DialogButtonsEnum.OkCancel:
				case DialogButtonsEnum.YesNoCancel:
					return DialogResultEnum.Cancel;
			}
			return DialogResultEnum.Undefined;
		}

		public readonly DialogButtonsEnum DialogButtons;
		private DialogResultEnum _dialogResult;

		public Visibility OkButtonVisibility { get; set; }
		public Visibility CancelButtonVisibility { get; set; }
		public Visibility YesButtonVisibility { get; set; }
		public Visibility NoButtonVisibility { get; set; }

		public ICommand OkCommand
		{
			get
			{
				return _okCommand ?? (_okCommand = new RelayCommand(param => OnButtonClicked(DialogResultEnum.Ok)));
			}
		}

		public ICommand CancelCommand
		{
			get
			{
				return _cancelCommand ?? (_cancelCommand= new RelayCommand(param => OnButtonClicked(DialogResultEnum.Cancel)));
			}
		}

		public ICommand YesCommand
		{
			get
			{
				return _yesCommand ?? (_yesCommand = new RelayCommand(param => OnButtonClicked(DialogResultEnum.Yes)));
			}
		}

		public ICommand NoCommand
		{
			get
			{
				return _noCommand ?? (_noCommand = new RelayCommand(param => OnButtonClicked(DialogResultEnum.No)));
			}
		}

		public void OnButtonClicked(DialogResultEnum dialogResult)
		{
			DialogResult = dialogResult;
			Close();
		}

		private void SubscribeToCloseEvent()
		{
			_childViewModel.RequestViewClose += DefaultDialogViewModel_RequestViewClose;
		}

		private void DefaultDialogViewModel_RequestViewClose(object sender, EventArgs e)
		{
			var model = ChildViewModel as IDialogResultProvider;
			if (model != null)
				DialogResult = model.DialogResult;
			Close();
		}

		private void SetButtonVisibilities()
		{
			OkButtonVisibility = Visibility.Collapsed;
			CancelButtonVisibility = Visibility.Collapsed;
			YesButtonVisibility = Visibility.Collapsed;
			NoButtonVisibility = Visibility.Collapsed;

			switch (DialogButtons)
			{
				case DialogButtonsEnum.None:
					break;
				case DialogButtonsEnum.Ok:
					OkButtonVisibility = Visibility.Visible;
					break;
				case DialogButtonsEnum.Cancel:
					CancelButtonVisibility = Visibility.Visible;
					break;
				case DialogButtonsEnum.OkCancel:
					OkButtonVisibility = Visibility.Visible;
					CancelButtonVisibility = Visibility.Visible;
					break;
				case DialogButtonsEnum.YesNo:
					YesButtonVisibility = Visibility.Visible;
					NoButtonVisibility = Visibility.Visible;
					break;
				case DialogButtonsEnum.YesNoCancel:
					YesButtonVisibility = Visibility.Visible;
					NoButtonVisibility = Visibility.Visible;
					CancelButtonVisibility = Visibility.Visible;
					break;
			}
		}
	}
}
