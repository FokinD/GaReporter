using System;

namespace GaReporter
{
	public class ReOpenDocumentCommand : Tools.CommandExtension<ReOpenDocumentCommand>
	{
		public override void Execute(object parameter)
		{
			try {
				((DocumentView)parameter).SetData(Tools.JsonIO.Open<Document>(GaReporter.Properties.Settings.Default.fileName));
			} catch (Exception) { }
		}
		
		public override bool CanExecute(object parameter)
		{
			return parameter is DocumentView;
		}
	}
	
	
}