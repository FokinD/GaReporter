using System;
using System.Data.Common;
using System.Windows;
using System.Windows.Data;
using Tools;
using System.Linq;
using System.Data;
using GaReporter.Properties;

namespace GaReporter
{
	public class RunRequestCommand : CommandExtension<RunRequestCommand>
	{
		public override void Execute(object parameter)
		{
			//определить что за элемент: документ, папка запрос или список запросов
			
			var doc = (DocumentView)((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data;
			var folder = (FolderView)doc.FoldersView.CurrentItem;
			var request = (RequestView)parameter;
			
			var keyPath = Tools.JsonIO.DefaultIfEmptyDir(doc.KeyPath);
            var accountEmailAddress = doc.AccountEmailAddress;
			var ids = doc.IDs;
			
			
			var title = request.Title;
			var fileName = request.FileName;

			var applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			
			var startDate = String.IsNullOrEmpty(request.StartDate) ? folder.StartDate : request.StartDate;
			var endDate = String.IsNullOrEmpty(request.EndDate) ? folder.EndDate : request.EndDate;
			var metrics = String.IsNullOrEmpty(request.Metrics) ? folder.Metrics : request.Metrics;
			var dimensions = String.IsNullOrEmpty(request.Dimensions) ? folder.Dimensions : request.Dimensions;
			var filters = String.IsNullOrEmpty(request.Filters) ? folder.Filters : request.Filters;
			var segment = String.IsNullOrEmpty(request.Segment) ? folder.Segment : request.Segment;
			var sort = String.IsNullOrEmpty(request.Sort) ? folder.Sort : request.Sort;
			var maxResults = request.MaxResults;
			
			bool desample = request.Desample;


			try {
				GetData(title, fileName, keyPath, accountEmailAddress, applicationName, ids, startDate, endDate, metrics, dimensions, filters, segment, sort, maxResults, desample);
				MessageBox.Show(string.Format("{0} {1} {2}", Resources.RefreshSuccessMessage0, request.Title, Resources.RefreshSuccessMessage1), string.Format("{0} {1}", Application.Current.MainWindow.Title, request.Title), MessageBoxButton.OK, MessageBoxImage.Information);

			} catch (Exception ex) {
				MessageBox.Show(string.Format("{0} {1}.\n\n{2}", Resources.RefreshErrorMessage, request.Title, ex.Message), string.Format("{0} {1}", Application.Current.MainWindow.Title, request.Title), MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		public override bool CanExecute(object parameter)
		{
			return System.Windows.Application.Current.MainWindow.DataContext is DataSourceProvider
				&& ((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data is DocumentView
				&& parameter is RequestView;
		}

		public static DataTable GetData(string tableName, string fileName, string keyPath, string accountEmailAddress, string applicationName, string ids, string startDate, string endDate, string metrics, string dimensions, string filters, string segment, string sort, int? maxResults, bool desample)
		{
			var data = GaAccess
				.GetGaData(keyPath, accountEmailAddress, applicationName, ids, startDate, endDate, metrics, dimensions, filters, segment, sort, maxResults, desample)
				.ToDataTable(tableName, dimensions.Split(',').Count());
			
			bool saved = String.IsNullOrEmpty(fileName);
			
			while (!saved) {
				try {
					data.SaveAs(fileName);
					saved = true;
					
				} catch (Exception ex) {
					if (System.Windows.MessageBox.Show(ex.Message, Resources.SaveErrorMessage, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.No) == System.Windows.MessageBoxResult.No) {
						throw;
					}
				}
			}
			
			return data;
		}
	}
}