using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;

namespace Tools
{
	public static class GetData
	{
		const char delim = ';';

		public static DataTable FromExcel(string workbook, string worksheet, IEnumerable<string> columns)
		{
			var dt = new DataTable();
			dt.TableName = worksheet;
			
			using (var connection = new OdbcConnection("DSN=Excel Files;DBQ=" + workbook))
			{
				var command = new OdbcCommand("SELECT " + (columns == null ? "*" : String.Join(",", columns.Select(e => "[" + e + "]"))) +
				                              " FROM [" + worksheet + "$]",
				                              connection);
				connection.Open();
				
				using (var reader = command.ExecuteReader())
					dt.Load(reader);
				
				connection.Close();
			}

			return dt;
		}
		public static DataTable FromCSV(string strFilePath)
		{
			var dt = new DataTable();
			using (var sr = new StreamReader(strFilePath, Encoding.Default)) {
				
				string[] headers = sr.ReadLine().Replace("\"", String.Empty).Split(delim);
				foreach (string header in headers)
				{
					dt.Columns.Add(header);
				}
				while (!sr.EndOfStream)
				{
					var t1 = sr.ReadLine();
					
					string[] rows = ParseCsvRow(t1);
					DataRow dr = dt.NewRow();
					for (int i = 0; i < headers.Length; i++)
					{
						dr[i] = rows[i];
					}
					dt.Rows.Add(dr);
				}
			}
			return dt;
		}
		
		public static string[] ParseCsvRow(string r)
		{

			string[] c;
			var resp = new List<string>();
			bool cont = false;
			string cs = "";

			c = r.Split(new char[] { delim }, StringSplitOptions.None);

			foreach (string y in c)
			{
				string x = y;


				if (cont)
				{
					// End of field
					if (x.EndsWith("\""))
					{
						cs += "," + x.Substring(0, x.Length - 1);
						resp.Add(cs);
						cs = "";
						cont = false;
						continue;

					}
					else
					{
						// Field still not ended
						cs += "," + x;
						continue;
					}
				}

				// Fully encapsulated with no comma within
				if (x.StartsWith("\"") && x.EndsWith("\""))
				{
					if ((x.EndsWith("\"\"") && !x.EndsWith("\"\"\"")) && x != "\"\"")
					{
						cont = true;
						cs = x;
						continue;
					}

					resp.Add(x.Substring(1, x.Length - 2));
					continue;
				}

				// Start of encapsulation but comma has split it into at least next field
				if (x.StartsWith("\"") && !x.EndsWith("\""))
				{
					cont = true;
					cs += x.Substring(1);
					continue;
				}

				// Non encapsulated complete field
				resp.Add(x);

			}

			return resp.ToArray();

		}
	}
}
