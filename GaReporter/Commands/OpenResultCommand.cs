using System;
using GaReporter.Properties;

namespace GaReporter
{
	public class OpenResultCommand : Tools.CommandExtension<OpenResultCommand>
	{

		public override void Execute(object parameter)
		{
			if (parameter is RequestView) {
				var request = (RequestView)parameter;
				try {
					System.Diagnostics.Process.Start(Tools.JsonIO.DefaultIfEmptyDir(request.FileName));
				} catch (Exception ex) {
					System.Windows.MessageBox
						.Show(string.Format("{0} {1}.\n\n{2}",
						Resources.RefreshErrorMessage,
						request.Title, ex.Message),
						string.Format("{0} {1}", System.Windows.Application.Current.MainWindow.Title, request.Title),
						System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				}
			} else {
				var folder = (FolderView)parameter;
				try {
					System.Diagnostics.Process.Start(Tools.JsonIO.DefaultIfEmptyDir(folder.FileName));
				} catch (Exception ex) {
					System.Windows.MessageBox
						.Show(string.Format("{0} {1}.\n\n{2}",
						Resources.RefreshErrorMessage,
						folder.Title, ex.Message),
						string.Format("{0} {1}", System.Windows.Application.Current.MainWindow.Title, folder.Title),
						System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				}
				
				//FIXME для раздела открывать файл раздела
			}
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is RequestView || parameter is FolderView;
		}
	}
}