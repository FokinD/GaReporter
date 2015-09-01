using System;

namespace GaReporter
{
	public class AddNewRequestCommand : Tools.CommandExtension<AddNewRequestCommand>
	{
		public override void Execute(object parameter)
		{
			var folder = (FolderView)parameter;
			var requests = folder.RequestsView;
			var request = new RequestView();

			folder.Requests.Insert(requests.CurrentPosition + 1, request);
			requests.MoveCurrentTo(request);
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is FolderView;
		}
	}
}
