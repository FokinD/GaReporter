using System;
using System.Data;
using System.IO;
using System.Windows;
using ClosedXML.Excel;
using System.Linq;
using Newtonsoft.Json;

namespace GaReporter
{
	public static class DataTableExtensionMethods
	{
		/// <summary>Сохраняет в нужном формате в зависимости от расширения файла</summary>
		public static void SaveAs(this DataTable dataTable, string fileName)
		{
			var ext = System.IO.Path.GetExtension(fileName);
			
			switch (ext) {
				case ".xml":
					dataTable.WriteXml(fileName);
					break;
					
				case ".xlsx":
					dataTable.SaveToExcel(fileName);
					break;
					
				case ".json":
					dataTable.SaveToJson(fileName);
					break;
					
				case ".csv": case ".txt":
					dataTable.SaveToCsv(fileName);
					break;
					
				default:
					throw new ArgumentException(string.Format(Properties.Resources.DataTableSaveAsArgumentException, "*.xml, *.xlsx, *.json, *.csv, *.txt"), "fileName");
			}
		}
		
		private static void SaveToExcel(this DataTable dataTable, string fileName)
		{
			var wb = new XLWorkbook();
			var dtName = dataTable.TableName;
			var name = dtName.Length <= 30 ? dtName : dtName.Substring(0, 30);
			wb.AddWorksheet(dataTable, name);
			try {
				wb.SaveAs(fileName);
				
			} catch (Exception ex) {
				MessageBox.Show(ex.Message); }
		}
		
		private static void SaveToJson(this DataTable dataTable, string fileName)
		{
			string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
			using (var outfile = new StreamWriter(fileName))
			{
				outfile.Write(json);
			}
		}
		
		private static void SaveToCsv(this DataTable dataTable, string fileName)
		{
			throw new NotImplementedException();
		}
	}
}