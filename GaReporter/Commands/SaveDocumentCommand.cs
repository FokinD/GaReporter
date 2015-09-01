using System;

namespace GaReporter
{
	public class SaveDocumentCommand : Tools.CommandExtension<SaveDocumentCommand>
	{
		public override void Execute(object parameter)
		{
			var fileName = GaReporter.Properties.Settings.Default.fileName;
			
			if (String.IsNullOrEmpty(fileName)) {
				(new SaveAsDocumentCommand()).Execute(parameter);
			} else {
				Tools.JsonIO.Save(((DocumentView)parameter).GetData(), fileName);
			}
		}
		
		public override bool CanExecute(object parameter)
		{
			return parameter is DocumentView;
		}
	}
}