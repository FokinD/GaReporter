using System;
using Tools;

namespace GaReporter
{
	public class FolderDetailsCommand : CommandExtension<FolderDetailsCommand>
	{
		public override void Execute(object parameter)
		{
			var w = System.Windows.Application.Current.MainWindow;
			
			(new FolderWindow { DataContext = w.DataContext, Owner = w,
				Title = string.Format(Properties.Resources.FoldersTitle, w.Title) }).Show();
		}
	}
}
