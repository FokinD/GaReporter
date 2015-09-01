using GaReporter.Properties;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Tools;

namespace GaReporter
{
	public class RunFolderCommand : Tools.CommandExtension<RunFolderCommand>
	{
		public static DateTime GetDateByWeek(string week)
		{
			var date = (new DateTime(int.Parse(week.Substring(0, 4)), 1, 1)).AddDays(int.Parse(week.Substring(2, 2)) * 7);
			var mon = date.AddDays(-(int)date.AddDays(-3).DayOfWeek - 2);
			
			return mon;
		}
		
		public override void Execute(object parameter)
		{
			var doc = (DocumentView)((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data;
			var folder = (FolderView)parameter;
			
			var keyPath = doc.KeyPath;
			var accountEmailAddress = doc.AccountEmailAddress;
			var ids = doc.IDs;

			var requests = folder.Requests;
			var fileName = folder.FileName;
			var applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			
			//загрузить весь DataSet
			DataSet ds = System.IO.File.Exists(fileName) ? JsonIO.Open<DataSet>(fileName) : new DataSet();

			try {
				foreach (var request in requests) {
					
					var title = request.Title;

					var startDate = String.IsNullOrEmpty(request.StartDate) ? folder.StartDate : request.StartDate;
					var endDate = String.IsNullOrEmpty(request.EndDate) ? folder.EndDate : request.EndDate;
					var metrics = String.IsNullOrEmpty(request.Metrics) ? folder.Metrics : request.Metrics;
					var dimensions = String.IsNullOrEmpty(request.Dimensions) ? folder.Dimensions : request.Dimensions;
					var filters = String.IsNullOrEmpty(request.Filters) ? folder.Filters : request.Filters;
					var segment = String.IsNullOrEmpty(request.Segment) ? folder.Segment : request.Segment;
					var sort = String.IsNullOrEmpty(request.Sort) ? folder.Sort : request.Sort;
					var maxResults = request.MaxResults;

					bool desample = folder.Desample || request.Desample;
					
					//выбрать таблицу, если пустая - потом просто добавим, если непустая, то обработаем
					var str_isoYearIsoWeek = "isoYearIsoWeek";
					
					if (ds.Tables.Contains(title)) {
						//Обрабатываем
						var dt = ds.Tables[title];
					
						//определить актуальность таблицы по последней неделе
						//определяем год и номер последней недели
						var lastYWeek = dt.AsEnumerable().LastOrDefault().Field<string>(str_isoYearIsoWeek);

						//определяем понедельник предыдущей недели
						var lastDate = GetDateByWeek(lastYWeek).AddDays(-7);
						
					} else {
						//просто выполняем и добавляем
						ds.Tables.Add(RunRequestCommand.GetData(title, request.FileName, keyPath, accountEmailAddress, applicationName, ids, startDate, endDate, metrics, dimensions, filters, segment, sort, maxResults, desample));
					}
					
				}
				
				//после выполнения всего раздела, если название файла не пустое, то объединить все файлы в один
				if (!String.IsNullOrEmpty(fileName)) {
					ds.SaveAs(fileName);
				}
				
				MessageBox.Show(string.Format("{0} {1} {2}", Resources.RefreshSuccessMessage0, folder.Title, Resources.RefreshSuccessMessage1), string.Format("{0} {1}", Application.Current.MainWindow.Title, folder.Title), MessageBoxButton.OK, MessageBoxImage.Information);

			} catch (Exception ex) {
				MessageBox.Show(string.Format("{0} {1}.\n\n{2}", Resources.RefreshErrorMessage, folder.Title, ex.Message), string.Format("{0} {1}", Application.Current.MainWindow.Title, folder.Title), MessageBoxButton.OK, MessageBoxImage.Error); }

		}

		public override bool CanExecute(object parameter)
		{
			return System.Windows.Application.Current.MainWindow.DataContext is DataSourceProvider
				&& ((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data is DocumentView
				&& parameter is FolderView;
		}
	}
}