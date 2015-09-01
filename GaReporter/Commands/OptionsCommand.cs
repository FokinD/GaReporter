using System;
using Tools;

namespace GaReporter
{
	public class OptionsCommand : CommandExtension<OptionsCommand>
	{
		public override void Execute(object parameter)
		{
			var w = System.Windows.Application.Current.MainWindow;
			var doc = (DocumentView)parameter;
			
			(new OptionsWindow() {
			 	DataContext = doc,
			 	Owner = w,
			 	Title = string.Format(Properties.Resources.OptionsTitle, w.Title)
			 }).ShowDialog();
		}
		
		public override bool CanExecute(object parameter)
		{
			return parameter is DocumentView;
		}
	}
}