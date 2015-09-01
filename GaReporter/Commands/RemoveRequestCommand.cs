using System;
using Tools;

namespace GaReporter
{
	public class RemoveRequestCommand : CommandExtension<RemoveRequestCommand>
	{
		public override void Execute(object parameter)
		{
			var folder = (FolderView)parameter;
			var request = (RequestView)folder.RequestsView.CurrentItem;
			
			folder.Requests.Remove(request);
			
			if (folder.Requests.Count == 0) {
				(new AddNewRequestCommand()).Execute(folder);
			}
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is FolderView && ((FolderView)parameter).RequestsView.CurrentItem != null;
		}
	}
}
