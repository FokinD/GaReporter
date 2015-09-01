using System;
using System.Text;
using Newtonsoft.Json;

namespace Tools
{
	public static class JsonIO
	{
		public static T Open<T>(string fileName)
		{
			var json = String.Empty;
			using (var reader = new System.IO.StreamReader(fileName)) {
				
				json = reader.ReadToEnd();
				reader.Close();
			}
			
			return JsonConvert.DeserializeObject<T>(json);
		}
		
		public static object Open(string fileName)
		{
			var json = String.Empty;
			using (var reader = new System.IO.StreamReader(fileName)) {
				
				json = reader.ReadToEnd();
				reader.Close();
			}
			
			return JsonConvert.DeserializeObject(json);
		}
		
		public static void Save(object obj, string fileName)
		{
			System.IO.File.WriteAllText(fileName, JsonConvert.SerializeObject(obj, Formatting.Indented), Encoding.UTF8);
		}
	}
}
