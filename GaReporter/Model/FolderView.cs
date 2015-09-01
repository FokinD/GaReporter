using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;

namespace GaReporter
{
	public class FolderView : INotifyPropertyChanged, ICloneable
	{
		public FolderView()
		{
			RequestsView = new ListCollectionView(new ObservableCollection<RequestView>());
		}

		public FolderView(Folder data) : this()
		{
			SetData(data);
		}

		public Folder GetData()
		{
			return new Folder {
				title = Title,
				file = FileName,
				start = StartDate,
				end = EndDate,
				metrics = Metrics,
				dimensions = Dimensions,
				filters = Filters,
				segment = Segment,
				sort = Sort,
				max = MaxResults ?? 0,
				desample = Desample,
				requests = Requests.Select(e => e.GetData()).ToArray()
			};
		}
		
		public void SetData(Folder value)
		{
			Title = value.title;
			FileName = value.file;
			StartDate = value.start;
			EndDate = value.end;
			Metrics = value.metrics;
			Dimensions = value.dimensions;
			Filters = value.filters;
			Segment = value.segment;
			Sort = value.sort;
			MaxResults = value.max == 0 ? null : (int?)value.max;
			Desample = value.desample;
			
			var requests = Requests;
			requests.Clear();
			
			foreach (var element in value.requests) {
				requests.Add(new RequestView(element));
			}

			RequestsView.MoveCurrentToFirst();
		}

		private string _Title = string.Empty;

		public string Title {
			get {
				return _Title;
			}
			set {
				if (_Title == value)
					return;
				_Title = value;
				OnPropertyChanged("Title");
			}
		}

		private string _FileName = string.Empty;

		public string FileName {
			get {
				return _FileName;
			}
			set {
				if (_FileName == value)
					return;
				_FileName = value;
				OnPropertyChanged("FileName");
			}
		}

		private string _StartDate = string.Empty;

		public string StartDate {
			get {
				return _StartDate;
			}
			set {
				if (_StartDate == value)
					return;
				_StartDate = value;
				OnPropertyChanged("StartDate");
			}
		}

		private string _EndDate = string.Empty;

		public string EndDate {
			get {
				return _EndDate;
			}
			set {
				if (_EndDate == value)
					return;
				_EndDate = value;
				OnPropertyChanged("EndDate");
			}
		}

		private string _Metrics = string.Empty;

		public string Metrics {
			get {
				return _Metrics;
			}
			set {
				if (_Metrics == value)
					return;
				_Metrics = value;
				OnPropertyChanged("Metrics");
			}
		}

		private string _Dimensions = string.Empty;

		public string Dimensions {
			get {
				return _Dimensions;
			}
			set {
				if (_Dimensions == value)
					return;
				_Dimensions = value;
				OnPropertyChanged("Dimensions");
			}
		}

		private string _Filters = string.Empty;

		public string Filters {
			get {
				return _Filters;
			}
			set {
				if (_Filters == value)
					return;
				_Filters = value;
				OnPropertyChanged("Filters");
			}
		}

		private string _Segment = string.Empty;

		public string Segment {
			get {
				return _Segment;
			}
			set {
				if (_Segment == value)
					return;
				_Segment = value;
				OnPropertyChanged("Segment");
			}
		}

		private string _Sort = string.Empty;

		public string Sort {
			get {
				return _Sort;
			}
			set {
				if (_Sort == value)
					return;
				_Sort = value;
				OnPropertyChanged("Sort");
			}
		}

		private int? _MaxResults = null;

		public int? MaxResults {
			get {
				return _MaxResults;
			}
			set {
				if (_MaxResults == value)
					return;
				_MaxResults = value;
				OnPropertyChanged("MaxResults");
			}
		}

		private bool _Desample = false;

		public bool Desample {
			get {
				return _Desample;
			}
			set {
				if (_Desample == value)
					return;
				_Desample = value;
				OnPropertyChanged("Desample");
			}
		}

		/// <summary>Список запросов папки</summary>
		public ListCollectionView RequestsView {
			get;
			private set;
		}

		public ObservableCollection<RequestView> Requests {
			get {
				return (ObservableCollection<RequestView>)RequestsView.SourceCollection;
			}
		}

		#region ICloneable implementation
		public object Clone()
		{
			return new FolderView(GetData());
		}

		#endregion
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}