using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Analytics.v3.Data;

namespace Tools
{
	public static class GaDataArrayExtensionMethods
	{
		private static Type GaDataToDataColumnDataType(string gaDataType)
		{
			switch (gaDataType) {
					case "DOUBLE": case "CURRENCY": case "TIME": case "PERCENT": case "FLOAT": return typeof(double);
					case "INTEGER": return typeof(int);
					case "STRING": return typeof(string);

					//только в режиме отладки
					default: throw new NotSupportedException(string.Format("Ga.DataType {0}", gaDataType));
			}
		}

		private static object GaDataConvert(string element, string gaDataType)
		{
			switch (gaDataType) {
					case "DOUBLE": case "CURRENCY": case "TIME": case "PERCENT": case "FLOAT": return double.Parse(element, System.Globalization.CultureInfo.InvariantCulture);
					case "INTEGER": return int.Parse(element);
					case "STRING": return element;
					
					default: throw new NotSupportedException(string.Format("Ga.DataType {0}", gaDataType));
			}
		}

		public static DataTable ToDataTable(this GaData[] gaData, string tableName)
		{
			const string str_sampleRate = "sampleRate";
			
			var gaCols = gaData[0].ColumnHeaders;
			var dt = new DataTable();
			var cols = dt.Columns;
			
			cols.AddRange(gaCols.Select(e => new DataColumn(e.Name.Substring(3)) {
			                            	ColumnMapping = MappingType.Attribute,
			                            	DataType = GaDataToDataColumnDataType(e.DataType)
			                            }).ToArray());
			
			var count = cols.Count;
			cols.Add(new DataColumn(str_sampleRate, typeof(double)) { ColumnMapping = MappingType.Attribute });
			
			var q = gaData.SelectMany(d => d.Rows.Select(e => {
			                                             	var row = dt.NewRow();
			                                             	for (int i = 0; i < count; i++)
			                                             		row[i] = GaDataConvert(e[i], gaCols[i].DataType);
			                                             	
			                                             	row[str_sampleRate] = (d.ContainsSampledData ?? false) ? ((double)d.SampleSize / (double)d.SampleSpace) : (double)1;
			                                             	return row;
			                                             }));
			
			q.CopyToDataTable(dt, LoadOption.OverwriteChanges);
			dt.TableName = tableName;

			return dt;
		}
		
		private class ArrayOfStringComparer : IEqualityComparer<string[]>
		{
			private static ArrayOfStringComparer _Default = new ArrayOfStringComparer();
			
			public static ArrayOfStringComparer Default {
				get { return _Default; }
			}
			
			#region IEqualityComparer implementation
			public bool Equals(string[] x, string[] y)
			{
				var res = x.SequenceEqual(y);
				return res;
			}
			
			public int GetHashCode(string[] obj)
			{
				/*int hash = 1;
				foreach (var element in obj) {
					hash = hash * element.GetHashCode();
				}*/
				return 0;
			}
			#endregion
		}

		public static DataTable ToDataTable(this GaData[] gaData, string tableName, int dimensions)
		{
			const string str_sampleRate = "sampleRate";
			
			var gaCols = gaData[0].ColumnHeaders;
			var dt = new DataTable();
			var cols = dt.Columns;
			
			int dims = dimensions;
			
			cols.AddRange(gaCols.Select(e => new DataColumn(e.Name.Substring(3)) {
			                            	ColumnMapping = MappingType.Attribute,
			                            	DataType = GaDataToDataColumnDataType(e.DataType)
			                            }).ToArray());
			
			var count = cols.Count;
			cols.Add(new DataColumn(str_sampleRate, typeof(double)) { ColumnMapping = MappingType.Attribute });
			
			var q = gaData
				.SelectMany(e => e.Rows.Select(r => { var res = new {
				                               		dimensions = r.Take(dims).ToArray(),
				                               		metrics = r.Skip(dims).Select(m => double.Parse(m, System.Globalization.CultureInfo.InvariantCulture)).ToArray(),
				                               		sample = (e.ContainsSampledData ?? false) ? ((double)e.SampleSize / (double)e.SampleSpace) : (double)1
				                               	};
				                               	return res; }))
				.GroupBy(e => e.dimensions, (IEqualityComparer<string[]>)ArrayOfStringComparer.Default)
				.Select(gr => {
				        	var row = dt.NewRow();
				        	
				        	for (int i = 0; i < count; i++) {
				        		row[i] = i < dims
				        			? (object)gr.Key[i]
				        			: (object)gr.Sum(e => e.metrics[i - dims]);
				        	}
				        	
				        	row[str_sampleRate] = gr.Average(e => e.sample);
				        	
				        	return row;
				        });
			
			q.CopyToDataTable(dt, LoadOption.OverwriteChanges);
			dt.TableName = tableName;

			return dt;
		}
	}
}