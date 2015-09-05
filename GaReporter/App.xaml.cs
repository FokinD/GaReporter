using System;
using System.Windows;

namespace GaReporter
{
	public partial class App : Application {
		
		protected override void OnStartup(StartupEventArgs e)
		{
			System.IO.Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
			
			
			if (e.Args.Length == 0) {
				try {
					
					DocumentView.GetInstance().SetData(Tools.JsonIO.Open<Document>(GaReporter.Properties.Settings.Default.fileName));
					
				} catch (Exception) { }
				
			} else {
				throw new NotImplementedException();
			}
			base.OnStartup(e);
		}
	}
}