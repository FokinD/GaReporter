using System;

namespace GaReporter
{
	public class Request
	{
		public string title {
			get;
			set;
		}

		public string file {
			get;
			set;
		}

		public string start {
			get;
			set;
		}

		public string end {
			get;
			set;
		}

		public string metrics {
			get;
			set;
		}

		public string dimensions {
			get;
			set;
		}

		public string filters {
			get;
			set;
		}

		public string segment {
			get;
			set;
		}

		public string sort {
			get;
			set;
		}

		public int? max {
			get;
			set;
		}

        public bool desample
        {
            get;
            set;
        }

        public bool full
        {
            get;
            set;
        }
    }
}