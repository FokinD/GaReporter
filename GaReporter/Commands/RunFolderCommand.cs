using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using Tools;

namespace GaReporter
{
    public class RunFolderCommand : Tools.CommandExtension<RunFolderCommand>
    {
        const string datePattern = "yyyy-MM-dd";

        public static DateTime GetDateByWeek(string week)
        {
            //var date = (new DateTime(int.Parse(week.Substring(0, 4)), 1, 1)).AddDays(int.Parse(week.Substring(4, 2)) * 7);
            //var mon = date.AddDays(-(int)date.AddDays(-3).DayOfWeek - 2);

            //return mon;

            return FirstDateOfWeekISO8601(int.Parse(week.Substring(0, 4)), int.Parse(week.Substring(4)));
        }

        public int WeekNumber(DateTime fromDate)
        {
            // Получаем 1 января указанного нами года
            DateTime startOfYear = fromDate.AddDays(-fromDate.Day + 1).AddMonths(-fromDate.Month + 1);
            // Получение 31 декабря указанного нами года
            DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
            //Согласно ISO 8601 четверг считается
            //четвёртым днём недели, а также днём,
            //который определяет нумерацию недель:
            //первая неделя года определяется как неделя,
            //содержащая первый четверг года, и так далее.
            //Вносим соответствующие корректировки
            int[] iso8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
            int nds = fromDate.Subtract(startOfYear).Days +
                iso8601Correction[(int)startOfYear.DayOfWeek];
            int wk = nds / 7;

            switch (wk)
            {
                case 0:
                    // Возвращаем номер недели от 31 декабря предыдущего года
                    return WeekNumber(startOfYear.AddDays(-1));
                case 53:
                    // Если 31 декабря выпадает до четверга 1 недели следующего года
                    return endOfYear.DayOfWeek < DayOfWeek.Thursday ? 1 : wk;

                default: return wk;
            }
        }
        //See more at: http://www.csharpcoderr.com/2012/11/num-week.html#sthash.1eNBJTWt.dpuf

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public override void Execute(object parameter)
        {
            var doc = (DocumentView)((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data;
            var folder = (FolderView)parameter;

            try
            {
                var keyPath = Tools.JsonIO.DefaultIfEmptyDir(doc.KeyPath);
                var accountEmailAddress = doc.AccountEmailAddress;
                var ids = doc.IDs;

                var requests = folder.Requests;
                var fileName = folder.FileName;
                var applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

                //загрузить весь DataSet
                DataSet ds = System.IO.Path.GetExtension(fileName) == ".json" && System.IO.File.Exists(fileName) ? JsonIO.Open<DataSet>(fileName) : new DataSet();

                foreach (var request in requests)
                {

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

                    if (ds.Tables.Contains(title) && ds.Tables[title].Columns.Contains(str_isoYearIsoWeek))
                    {//только если в измерениях есть неделя, остальные случаи необходимо доработать
                     //Обрабатываем
                        var dt = ds.Tables[title];

                        //определить актуальность таблицы по последней неделе
                        //определяем год и номер последней недели
                        var lastIsoYearIsoWeek = dt.AsEnumerable().LastOrDefault().Field<string>(str_isoYearIsoWeek);

                        //определяем понедельник предыдущей недели
                        var lastDate = GetDateByWeek(lastIsoYearIsoWeek);
                        var prevDate = lastDate.AddDays(-7);
                        var prevIsoYearIsoWeek = string.Format("{0}{1}", prevDate.Year, WeekNumber(prevDate));

                        //удаляем лишние строки, где год-неделя соответствуют неактуальным датам
                        var rowsToRemove = dt.AsEnumerable()
                            .Where(e => e.Field<string>(str_isoYearIsoWeek) == prevIsoYearIsoWeek || e.Field<string>(str_isoYearIsoWeek) == lastIsoYearIsoWeek)
                            .ToArray();

                        foreach (var element in rowsToRemove)
                        {
                            dt.Rows.Remove(element);
                        }

                        //выполняем запрос начиная с актуальной даты
                        var dt2 = GaAccess
                            .GetGaData(keyPath, accountEmailAddress, applicationName, ids, prevDate.ToString(datePattern), endDate, metrics, dimensions, filters, segment, sort, maxResults, desample)
                            .ToDataTable(title, dimensions.Split(',').Count());

                        foreach (var element in dt2.AsEnumerable())
                        {
                            var row = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                row[i] = element[i];
                            }

                            dt.Rows.Add(row);
                        }

                        dt.AcceptChanges();


                    }
                    else
                    {
                        //просто выполняем и добавляем

                        ds.Tables.Add(GaAccess
                                      .GetGaData(keyPath, accountEmailAddress, applicationName, ids, startDate, endDate, metrics, dimensions, filters, segment, sort, maxResults, desample)
                                      .ToDataTable(title, dimensions.Split(',').Count()));

                    }

                }

                //после выполнения всего раздела, если название файла не пустое, то объединить все файлы в один
                if (!String.IsNullOrEmpty(fileName))
                {
                    ds.SaveAs(fileName);
                }

                MessageBox.Show(string.Format("{0} {1} {2}", Properties.Resources.RefreshSuccessMessage0, folder.Title, Properties.Resources.RefreshSuccessMessage1), string.Format("{0} {1}", Application.Current.MainWindow.Title, folder.Title), MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} {1}.\n\n{2}", Properties.Resources.RefreshErrorMessage, folder.Title, ex.Message), string.Format("{0} {1}", Application.Current.MainWindow.Title, folder.Title), MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public override bool CanExecute(object parameter)
        {
            return System.Windows.Application.Current.MainWindow.DataContext is DataSourceProvider
                && ((DataSourceProvider)System.Windows.Application.Current.MainWindow.DataContext).Data is DocumentView
                && parameter is FolderView;
        }
    }
}