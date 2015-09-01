using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace GaReporter
{
	public class AddNewFolderCommand : Tools.CommandExtension<AddNewFolderCommand>
	{
		public override void Execute(object parameter)
		{
			var folders = (ListCollectionView)parameter;
			var folder = new FolderView();

			((ObservableCollection<FolderView>)folders.SourceCollection).Insert(folders.CurrentPosition + 1, folder);
			folders.MoveCurrentTo(folder);
			
			(new AddNewRequestCommand()).Execute(folder);
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is ListCollectionView;
		}
	}
}