using SC.Base.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace SC.Base.WPF.Reporting
{
	public class ExceptionViewModel : ViewModelBase
	{
		private string _name = "";
		private Exception _exception;
		private ObservableCollection<ExceptionViewModel> _innerException;
		
		public ExceptionViewModel(Exception ex)
		{
			Exception = ex;
			if (ex != null)
			{
                InnerException =
                    new ObservableCollection<ExceptionViewModel> {new ExceptionViewModel(ex.InnerException)};
                Name = ex.GetType().FullName;
			}

		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				_exception = value;
				OnPropertyChanged(nameof(Exception));
			}
		}

		public ObservableCollection<ExceptionViewModel> InnerException
		{
			get
			{
				return _innerException;
			}
			set
			{
				_innerException = value;
				OnPropertyChanged(nameof(InnerException));
			}
		}
	}
}
