using System;

namespace GaReporter
{
	public class OpenResultCommand : Tools.CommandExtension<OpenResultCommand>
	{

		public override void Execute(object parameter)
		{
			//FIXME для раздела открывать файл раздела
			System.Diagnostics.Process.Start(((RequestView)parameter).FileName);
		}

		public override bool CanExecute(object parameter)
		{
			return parameter is RequestView;
		}
	}
}