using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace GaReporter
{
	public class CloneFolderCommand : Tools.CommandExtension<CloneFolderCommand>
	{
		public override void Execute(object parameter)
		{
			var folders = (ListCollectionView)parameter;
			var folder = (FolderView)((ICloneable)folders.CurrentItem).Clone();

			((ObservableCollection<FolderView>)folders.SourceCollection).Insert(folders.CurrentPosition + 1, folder);
			folders.MoveCurrentTo(folder);
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is ListCollectionView && ((ListCollectionView)parameter).CurrentItem != null;
		}
	}
}