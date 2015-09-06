using System;
using GaReporter.Properties;

namespace GaReporter
{
	public class OpenResultCommand : Tools.CommandExtension<OpenResultCommand>
	{

		public override void Execute(object parameter)
		{
            //FIXME для раздела открывать файл раздела
            var request = (RequestView)parameter;
            try
            {
                System.Diagnostics.Process.Start(request.FileName);
            }
            catch (Exception ex) {
                System.Windows.MessageBox
                    .Show(string.Format("{0} {1}.\n\n{2}",
                    Resources.RefreshErrorMessage,
                    request.Title, ex.Message),
                    string.Format("{0} {1}", System.Windows.Application.Current.MainWindow.Title, request.Title),
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

		public override bool CanExecute(object parameter)
		{
			return parameter is RequestView;
		}
	}
}