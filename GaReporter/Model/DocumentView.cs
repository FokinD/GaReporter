using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;

namespace GaReporter
{
    public class DocumentView : INotifyPropertyChanged, ICloneable, IDataErrorInfo
    {
        private static DocumentView _Instance = null;

        public static DocumentView GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new DocumentView();
            }
            return _Instance;
        }

        private DocumentView()
        {
            var folders = new ListCollectionView(new ObservableCollection<FolderView>());
            FoldersView = folders;
            (new AddNewFolderCommand()).Execute(folders);
        }

        private DocumentView(Document data) : this()
        {
            SetData(data);
        }

        public void Clear()
        {
            Version = String.Empty;
            KeyPath = String.Empty;
            AccountEmailAddress = String.Empty;
            IDs = String.Empty;

            Folders.Clear();
            (new AddNewFolderCommand()).Execute(FoldersView);
        }

        public Document GetData()
        {
            return new Document
            {
                version = Version,
                key = KeyPath,
                account = AccountEmailAddress,
                ids = IDs,
                folders = Folders.Select(e => e.GetData()).ToArray()
            };
        }

        public void SetData(Document value)
        {
            Version = value.version;
            KeyPath = value.key;
            AccountEmailAddress = value.account;
            IDs = value.ids;

            var folders = Folders;
            folders.Clear();

            if (value.folders != null)
            {
                foreach (var element in value.folders)
                {
                    folders.Add(new FolderView(element));
                }
            }

            FoldersView.MoveCurrentToFirst();
        }

        private string _Version = "1.0";

        /// <summary>Версия документа</summary>
        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                if (_Version == value)
                    return;
                _Version = value;
                OnPropertyChanged("Version");
            }
        }

        private string _KeyPath = String.Empty;

        /// <summary>Расположение ключа</summary>
        public string KeyPath
        {
            get
            {
                return _KeyPath;
            }
            set
            {
                if (_KeyPath == value)
                    return;
                _KeyPath = value;
                OnPropertyChanged("KeyPath");
            }
        }

        private string _AccountEmailAddress = String.Empty;

        /// <summary>Учётная запись в GA</summary>
        public string AccountEmailAddress
        {
            get
            {
                return _AccountEmailAddress;
            }
            set
            {
                if (_AccountEmailAddress == value)
                    return;
                _AccountEmailAddress = value;
                OnPropertyChanged("AccountEmailAddress");
            }
        }

        private string _IDs = String.Empty;

        /// <summary>Идентификатор</summary>
        public string IDs
        {
            get
            {
                return _IDs;
            }
            set
            {
                if (_IDs == value)
                    return;
                _IDs = value;
                OnPropertyChanged("IDs");
            }
        }

        /// <summary>Список запросов папки</summary>
        public ListCollectionView FoldersView
        {
            get;
            private set;
        }

        public ObservableCollection<FolderView> Folders
        {
            get
            {
                return (ObservableCollection<FolderView>)FoldersView.SourceCollection;
            }
        }

        #region IDataErrorInfo implementation
        public string this[string columnName]
        {
            get
            {
                string msg = null;
                switch (columnName)
                {
                    case "AccountEmailAddress":
                        break;

                    case "IDs":
                        break;

                    case "KeyPath":
                        break;
                }

                return msg;
            }
        }

        public string Error
        {
            get { return null; }
        }
        #endregion

        #region ICloneable implementation
        public object Clone()
        {
            return new DocumentView(GetData());
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