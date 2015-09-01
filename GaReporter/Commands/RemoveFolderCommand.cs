using System;
using System.Windows.Data;

namespace GaReporter
{
	public class RemoveFolderCommand : Tools.CommandExtension<RemoveFolderCommand>
	{
		public override void Execute(object parameter)
		{
			var folders = (ListCollectionView)parameter;
			folders.Remove(folders.CurrentItem);
			
			if (folders.Count == 0) {
				(new AddNewFolderCommand()).Execute(folders);
			}
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is ListCollectionView && ((ListCollectionView)parameter).CurrentItem != null;
		}
	}
}
