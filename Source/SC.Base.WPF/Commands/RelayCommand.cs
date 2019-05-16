using System;
using System.Windows.Input;
using System.Diagnostics;
using SC.Base.WPF.Reporting;

namespace SC.Base.WPF.Commands
{
	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Fields

		readonly Action<object> _execute;
		readonly Predicate<object> _canExecute;
		private Exception _error;
		#endregion // Fields

		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public RelayCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion // Constructors

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			try
			{
				return _canExecute?.Invoke(parameter) ?? true;
			}
			catch (Exception ex)
			{
				ReportUtility.Report(ex);
			}

			return false;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void Execute(object parameter)
		{
			try
			{
				if (!CanExecute(null))
					throw new Exception("Execute Command denied!");
				_execute(parameter);
			}
			catch (Exception ex)
			{
				_error = ex;
				ReportUtility.Report(ex);
			}
		}

		#endregion // ICommand Members

		public Exception GetError()
		{
			return _error;
		}

		public bool IsError()
		{
			return _error != null;
		}
	}
}
