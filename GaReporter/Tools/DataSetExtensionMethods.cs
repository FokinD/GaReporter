using System;
using System.Data;
using System.IO;
using System.Windows;
using ClosedXML.Excel;
using System.Linq;
using Newtonsoft.Json;

namespace GaReporter
{
	public static class DataSetExtensionMethods
	{
		/// <summary>Сохраняет в нужном формате в зависимости от расширения файла</summary>
		public static void SaveAs(this DataSet dataSet, string fileName)
		{
			var ext = System.IO.Path.GetExtension(fileName);
			fileName = Tools.JsonIO.DefaultIfEmptyDir(fileName);
			
			switch (ext) {
				case ".xml":
					dataSet.WriteXml(fileName);
					break;
				case ".xlsx":
					dataSet.SaveToExcel(fileName);
					break;
				case ".json":
					dataSet.SaveToJson(fileName);
					break;
				case ".csv":
				case ".txt":
					dataSet.SaveToCsv(fileName);
					break;
				default:
					throw new ArgumentException(string.Format(Properties.Resources.DataTableSaveAsArgumentException, "*.xml, *.xlsx, *.json, *.csv, *.txt"), "fileName");
			}
		}

		private static void SaveToExcel(this DataSet dataSet, string fileName)
		{
			var wb = new XLWorkbook();
			foreach (var dt in dataSet.Tables.Cast<DataTable>()) {
				var dtName = dt.TableName;
				var name = dtName.Length <= 30 ? dtName : dtName.Substring(0, 30);
				wb.AddWorksheet(dt, name);
			}
			try {
				wb.SaveAs(fileName);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private static void SaveToJson(this DataSet dataSet, string fileName)
		{
			string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
			using (var outfile = new StreamWriter(fileName)) {
				outfile.Write(json);
			}
		}

		private static void SaveToCsv(this DataSet dataSet, string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
