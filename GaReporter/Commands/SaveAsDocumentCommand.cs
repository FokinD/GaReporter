using System;
using System.Configuration;

namespace GaReporter
{
	public class SaveAsDocumentCommand : Tools.CommandExtension<SaveAsDocumentCommand>
	{
		public override void Execute(object parameter)
		{
			//сначала выбираем название файла
			//затем меняем в настройках
			//затем сохраняем
			
			var doc = (DocumentView)parameter;
			try {
				
				//проверить, есть ли у документа имя
				
				var fileName = GaReporter.Properties.Settings.Default.fileName;
				var dir = System.IO.File.Exists(fileName)
                    ? System.IO.Path.GetDirectoryName(fileName)
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				var fileDialog = new Microsoft.Win32.SaveFileDialog() {
					InitialDirectory = dir,
					FileName = System.IO.Path.GetFileName(fileName),
					DefaultExt = "*.json",
					Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
				};
				
				if(fileDialog.ShowDialog() == true)
				{
					fileName = fileDialog.FileName;
					Tools.JsonIO.Save(doc.GetData(), fileName);
					
					GaReporter.Properties.Settings.Default.fileName = fileName;
					GaReporter.Properties.Settings.Default.Save();
					
				} else { throw new NotImplementedException(); }
				
			} catch (Exception) { }
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is DocumentView;
		}
	}
}

