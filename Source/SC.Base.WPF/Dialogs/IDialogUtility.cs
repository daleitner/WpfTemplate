namespace SC.Base.WPF.Dialogs
{
	public interface IDialogUtility
	{
		DialogResultEnum Show(DialogObject dialogObject);
		DialogResultEnum Show(string message, string caption);
	}
}
