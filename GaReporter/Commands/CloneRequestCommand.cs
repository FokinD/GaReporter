using System;
using Tools;

namespace GaReporter
{
	public class CloneRequestCommand : CommandExtension<CloneRequestCommand>
	{
		public override void Execute(object parameter)
		{
			var folder = (FolderView)parameter;
			var requests = folder.RequestsView;
			var request = (RequestView)((ICloneable)requests.CurrentItem).Clone();
			
			folder.Requests.Insert(requests.CurrentPosition + 1, request);
			requests.MoveCurrentTo(request);
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is FolderView && ((FolderView)parameter).RequestsView.CurrentItem != null;
		}
	}
}

