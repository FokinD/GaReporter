using System;

namespace GaReporter
{
	public class Document
	{
		public string version {
			get;
			set;
		}

		public string key {
			get;
			set;
		}

		public string account {
			get;
			set;
		}

		public string ids {
			get;
			set;
		}

		public Folder[] folders {
			get;
			set;
		}
	}
}