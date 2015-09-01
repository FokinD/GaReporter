using System;
using System.Globalization;
using Google.Apis.Analytics.v3.Data;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Analytics.v3;

namespace Tools
{
	/// <summary>Должен обрабатывать только запросы к GA. Не должен никак интерпретировать данные, не должен сохранять или подгружать.</summary>
	public static class GaAccess
	{
		//TODO добавить кэширование
		const int max = 10000;
		const string datePattern = "yyyy-MM-dd";
		
		/// <summary>Только для запросов с известными maxResults и startIndex, выполняется один раз</summary>
		private static GaData GetGaData(AnalyticsService service, string ids, string startDate, string endDate, string metrics, string dimensions, string filters, string segment, string sort, int? maxResults, int? startIndex) {
			
			DataResource.GaResource.GetRequest request = service.Data.Ga.Get(ids, startDate, endDate, metrics);
			request.SamplingLevel = DataResource.GaResource.GetRequest.SamplingLevelEnum.HIGHERPRECISION;
			
			if (!string.IsNullOrEmpty(dimensions)) request.Dimensions = dimensions;
			if (!string.IsNullOrEmpty(filters)) request.Filters = filters;
			if (!string.IsNullOrEmpty(segment)) request.Segment = segment;
			if (!string.IsNullOrEmpty(sort)) request.Sort = sort;
			
			if (startIndex != null) request.StartIndex = startIndex;
			if (maxResults != null) request.MaxResults = Math.Min(max, (int)maxResults);
			
			var res = request.Execute();
			return res;
		}
		
		/// <summary>Умеет разбивать на запросы по 10000 строк, без десэмплирования</summary>
		private static GaData[] GetGaData(AnalyticsService service, string ids, string startDate, string endDate, string metrics, string dimensions, string filters, string segment, string sort, int? maxResults) {
			
			var data = new List<GaData>();
			var i = 0;
			var remain = maxResults;
			
			do {

				//FIXME обрабатывать ошибки запроса, пытаться выполнить как минимум 2 раза! желательно кэшировать выполнение, чтобы потом не извлекать повторно
				//для кэширования считаем хэш, добавляем период и некоторые другие условия, успешный результат сохраняем для последующего использования
				//перед обращением к запросу сначала смотрим кэш, затем формируем запрос исходя из наличия данных в кэше
				//устаревший (тот, к которому долгое время не обращались, а значит вести таблицу обращений к кэшу) кэш необходимо удалять
				//ошибку можно сбрасывать в отстойник, который отправить в конец очереди
				//ошибку имеет смысл ловить при нескольких запросах, либо более 10000 строк, либо при десэмплировании, либо при выполнении нескольких запросов одновременно
				var gd = GetGaData(service, ids, startDate, endDate, metrics, dimensions, filters, segment, sort, remain, i == 0 ? (int?)null : (int?)(i * max));
				data.Add(gd);
				remain = remain == null ? 0 : (gd.Rows != null && gd.Rows.Count == max) ? Math.Max(0, (int)remain - max) : 0;
				i++;

			} while (remain > 0);
			
			return data.ToArray();
		}

		/// <summary>Извлекает данные из Google Analytics без ограничения по количеству строк и может десэмплировать путем разбивки на даты, при необходимости</summary>
		/// <param name="maxResults">Без ограничения</param>
		/// <param name="desample">Если истина, то запрос будет разбит по датам</param>
		/// <returns></returns>
		public static GaData[] GetGaData(string keyPath, string accountEmailAddress, string applicationName, string ids, string startDate, string endDate, string metrics, string dimensions, string filters, string segment, string sort, int? maxResults, bool desample)
		{
			//сначала обрабатываеть необходимость десэмплирования
			IEnumerable<string[]> dates;

			if (desample) {
				//по требованию пользователя, диапазон разбивается на даты
				var start = DateTime.ParseExact(startDate, datePattern, CultureInfo.InvariantCulture);
				var end = endDate == "yesterday"
					? DateTime.Today.AddDays(-1)
					: (endDate == "today"
					   ? DateTime.Today
					   : DateTime.ParseExact(endDate, datePattern, CultureInfo.InvariantCulture));
				//TODO обработку нестандартных форматов (например daysago)
				
				var days = (int)(end - start).TotalDays + 1;
				
				dates = Enumerable
					.Range(0, days)
					.Select(e => start.AddDays(e).ToString(datePattern))
					.Select(e => new [] { e, e });

				
			} else {
				//не разбивается
				dates = new [] { new [] { startDate, endDate } };
			}

			
			GaData[] data;//извлекаются данные
			
			
			using (var service = AnalyticsServiceFactory.GetService(keyPath, accountEmailAddress, applicationName))//выполняется подключение
			{
				//FIXME обрабатывать возникшие ошибки, некоторые запросы пытаться выполнить повторно
				data = dates.SelectMany(e => GetGaData(service, ids, e[0], e[1], metrics, dimensions, filters, segment, sort, maxResults)).ToArray();//выполняются запросы
				//TODO группировать данные разбитые на даты в одну
			}
			
			return data;
		}
	}
}