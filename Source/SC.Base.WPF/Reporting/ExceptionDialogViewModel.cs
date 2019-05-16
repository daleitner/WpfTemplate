using System;
using SC.Base.WPF.ViewModels;
using SC.Base.WPF.Commands;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using SC.Base.WPF.Common;

namespace SC.Base.WPF.Reporting
{
	public class ExceptionDialogViewModel : WorkspaceViewModel
	{
		private readonly ExceptionDialogWindow _exceptionWindow;
		private ExceptionViewModel _rootException;
		private ExceptionViewModel _selectedRootException;
		private ObservableCollection<ExceptionViewModel> _rootExceptionList;
		private Exception _exceptionProps;
		private string _stackTrace;
		private Visibility _expandedVisibility = Visibility.Visible;


		public ExceptionDialogViewModel(Exception exc, ExceptionDialogWindow exceptionWindow)
		{
			_exceptionWindow = exceptionWindow;
			RootException = new ExceptionViewModel(exc);
			RootExceptionList = new ObservableCollection<ExceptionViewModel> { RootException };

			StackTrace = exc.StackTrace;
			ExceptionProps = exc;
		}

		public ExceptionViewModel RootException
		{
			get
			{
				return _rootException;
			}
			set
			{
				_rootException = value;
				OnPropertyChanged(nameof(RootException));
			}
		}

		public ExceptionViewModel SelectedRootException
		{
			get
			{
				return _selectedRootException;
			}
			set
			{
				_selectedRootException = value;
				if (value != null)
				{
					ExceptionProps = _selectedRootException.Exception;
					StackTrace = ExceptionProps.StackTrace;
				}
				OnPropertyChanged(nameof(SelectedRootException));
			}
		}

		public GridLength RowHeight
		{
			get
			{
				if (ExpandedVisibility == Visibility.Visible)
					return new GridLength(1, GridUnitType.Star);

				return new GridLength(0);
			}
		}

		public ObservableCollection<ExceptionViewModel> RootExceptionList
		{
			get
			{
				return _rootExceptionList;
			}
			set
			{
				_rootExceptionList = value;
				OnPropertyChanged(nameof(RootExceptionList));
			}
		}

		public Visibility ExpandedVisibility
		{
			get
			{
				return _expandedVisibility;
			}
			set
			{
				_expandedVisibility = value;
				OnPropertyChanged(nameof(ExpandedVisibility));
			}
		}

		public string StackTrace
		{
			get
			{
				return _stackTrace;
			}
			set
			{
				_stackTrace = value;
				OnPropertyChanged(nameof(StackTrace));
			}
		}

		public string ExceptionMessage => _exceptionProps.Message;

		public Exception ExceptionProps
		{
			get
			{
				return _exceptionProps;
			}
			set
			{
				_exceptionProps = value;
				OnPropertyChanged(nameof(ExceptionProps));
			}
		}

		#region CopyClipboardCommand

		private ICommand _copyClipboardCommand;
		public ICommand CopyClipboardCommand
		{
			get
			{
				return _copyClipboardCommand ?? (_copyClipboardCommand = new RelayCommand(
						param => CopyClipboard()
					));
			}
		}

		public void CopyClipboard()
		{
			Clipboard.SetText(GetErrorMessage());
		}
		#endregion

		#region ExpandCommand

		private ICommand _expandCommand;
		public ICommand ExpandCommand
		{
			get
			{
				return _expandCommand ?? (_expandCommand = new RelayCommand(
						param => Expand()
					));
			}
		}

		public void Expand()
		{
			int height = 600;
			double maxHeight = double.PositiveInfinity;

			if (ExpandedVisibility == Visibility.Collapsed)
			{
				ExpandedVisibility = Visibility.Visible;
			}
			else
			{
				ExpandedVisibility = Visibility.Collapsed;
				height = 160;
				maxHeight = 160;
			}

			OnPropertyChanged(nameof(RowHeight));

			_exceptionWindow.MaxHeight = maxHeight;
			_exceptionWindow.Height = height;

		}
		#endregion


		private string GetErrorMessage()
		{
			string exeptionMessage = RootException.Exception.ToString();
			string msg = "Application: " + EnvironmentInfo.Instance.ApplicationName + "\r\nVersion: " + EnvironmentInfo.Instance.Version + "\r\n\r\n" + exeptionMessage;

			return msg;
		}
	}
}
