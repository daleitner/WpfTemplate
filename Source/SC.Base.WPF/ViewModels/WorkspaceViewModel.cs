using System;
using SC.Base.WPF.Commands;
using System.Windows.Input;
using System.ComponentModel;
using log4net;
using SC.Base.WPF.Reporting;

namespace SC.Base.WPF.ViewModels
{
	/// <summary>
	/// This ViewModelBase subclass requests to be removed 
	/// from the UI when its CloseCommand executes.
	/// This class is abstract.
	/// </summary>
	public abstract class WorkspaceViewModel : ViewModelBase
	{
		#region Fields
		protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private RelayCommand _refreshCommand;
		private RelayCommand _closeCommand;
		private bool _isBusy;
		private string _busyContent = "Loading ...";
		private bool _isReadOnly;
		#endregion // Fields

		#region Constructor

		protected WorkspaceViewModel()
		{
		}


		#endregion // Constructor

		#region Refresh

		public bool RefreshSynchron { get; set; }

		public ICommand RefreshCommand
		{
			get
			{
				return _refreshCommand ?? (_refreshCommand = new RelayCommand(
						   param => Refresh(),
						   param => CanRefresh()
					   ));
			}
		}

		public void Refresh()
		{
			Refresh(false, null);
		}

		public virtual void Refresh(bool isInitialRefresh, object argument)
		{
			if (CanRefresh() == false)
				return;

			if (RefreshSynchron)
				OnRefreshSynchron(isInitialRefresh, argument);
			else
				OnRefreshAsynchron(isInitialRefresh, argument);
		}

		void loadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (e.Error == null)
					Update(e);
				else
				{
					ReportUtility.Report(e.Error);
				}
			}
			catch (Exception ex)
			{
				ReportUtility.Report(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void OnRefreshSynchron(bool isInitialLoad, object argument)
		{
			Exception errorException = null;
			var doWorkEventArgs = new DoWorkEventArgs(argument);
			try
			{
				LoadData(isInitialLoad, doWorkEventArgs);
			}
			catch (Exception ex)
			{
				errorException = ex;
			}
			Update(new RunWorkerCompletedEventArgs(doWorkEventArgs.Result, errorException,
				doWorkEventArgs.Cancel));
		}

		private void OnRefreshAsynchron(bool isInitialRefresh, object argument)
		{
			var loadingWorker = new BackgroundWorker();
			loadingWorker.RunWorkerCompleted += loadingWorker_RunWorkerCompleted;
			loadingWorker.DoWork += (o, ea) =>
			{
				//Sleep(1) for open GUI before starting loaddata.
				System.Threading.Thread.Sleep(1);
				LoadData(isInitialRefresh, ea);
			};

			IsBusy = true;
			loadingWorker.RunWorkerAsync(argument);
		}

		public virtual bool CanRefresh()
		{
			return true;
		}

		#endregion

		#region BusyIndicator

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				_isBusy = value;
				OnPropertyChanged(nameof(IsBusy));
			}
		}

		public string BusyContent
		{
			get
			{
				return _busyContent;
			}
			set
			{
				_busyContent = value;
				OnPropertyChanged(nameof(BusyContent));
			}
		}

		public virtual void LoadData(bool isInitialLoad, DoWorkEventArgs doWorkArgs)
		{
		}

		public virtual void Update(RunWorkerCompletedEventArgs completedArgs)
		{
		}

		#endregion

		#region IsReadOnly

		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(nameof(IsReadOnly));
			}

		}
		#endregion

		#region CloseCommand

		/// <summary>
		/// Returns the command that, when invoked, attempts
		/// to remove this workspace from the user interface.
		/// </summary>
		public ICommand CloseCommand
		{
			get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => OnRequestClose())); }
		}

		#endregion // CloseCommand

		#region RequestClose [event]

		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		public event EventHandler RequestClose;

		protected virtual void OnRequestClose()
		{
			RequestClose?.Invoke(this, EventArgs.Empty);

			Close();
		}
		#endregion // RequestClose [event]

		#region RequestCloseView [event]

		public event EventHandler RequestViewClose;

		public void Close()
		{
			RequestViewClose?.Invoke(this, EventArgs.Empty);
		}

		#endregion // RequestCloseView [event]

		#region protected methods
		/// <summary>
		/// Runs the function doWork in a <seealso cref="BackgroundWorker" />. After finishing doWork workCompleted is executed.
		/// If <seealso cref="RefreshSynchron" /> is set, then doWork is executed in Main-Thread.
		/// </summary>
		/// <param name="doWork">Function which is executed in Background. <seealso cref="RefreshSynchron" /> must be set to false.</param>
		/// <param name="workCompleted">Function which is executed in Main-Thread when doWork is done.</param>
		protected void RunAsync(DoWorkEventHandler doWork, RunWorkerCompletedEventHandler workCompleted)
		{
			if (RefreshSynchron)
			{
				var args = new DoWorkEventArgs(null);
				doWork(null, args);
				workCompleted(null, new RunWorkerCompletedEventArgs(args.Result, null, args.Cancel));
			}
			else
			{
				var worker = new BackgroundWorker();
				worker.RunWorkerCompleted += (o, e) =>
				{
					IsBusy = false;
					workCompleted(o, e);
				};
				worker.DoWork += doWork;
				IsBusy = true;
				worker.RunWorkerAsync();
			}
		}
		#endregion
	}
}
