using System;

namespace GaReporter
{
	public class OpenDocumentCommand : Tools.CommandExtension<OpenDocumentCommand>
	{
		public override void Execute(object parameter)
		{
			var doc = (DocumentView)parameter;
			
			try {
				var fileName = GaReporter.Properties.Settings.Default.fileName;
				string dir = String.Empty;
				try {

					dir = System.IO.Path.GetDirectoryName(fileName);

				} catch (Exception) {

					dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				}
				
				var fileDialog = new Microsoft.Win32.OpenFileDialog() {
					InitialDirectory = dir,
					DefaultExt = "*.json",
					Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
				};
				if (fileDialog.ShowDialog() == true) {
					fileName = fileDialog.FileName;
					doc.SetData(Tools.JsonIO.Open<Document>(fileName));
					
					GaReporter.Properties.Settings.Default.fileName = fileName;
					GaReporter.Properties.Settings.Default.Save();
				}
				else {
					throw new NotImplementedException();
				}
				((DocumentView)parameter).SetData(Tools.JsonIO.Open<Document>(GaReporter.Properties.Settings.Default.fileName));
			}
			catch (Exception) { }
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is DocumentView;
		}
	}
}

