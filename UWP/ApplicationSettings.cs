using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWP
{
    public static class ApplicationSettings
    {
        private const string FiltersFileName = "filters";

        private static ApplicationDataContainer localSettings;
        private static StorageFolder localFolder;
        private static ObservableCollection<string> filters;

        public static string SourceFolderPath
        {
            get { return (string)localSettings.Values[nameof(SourceFolderPath)]; }
            set { localSettings.Values[nameof(SourceFolderPath)] = value; }
        }
        public static string DestinationFolderPath
        {
            get { return (string)localSettings.Values[nameof(DestinationFolderPath)]; }
            set { localSettings.Values[nameof(DestinationFolderPath)] = value; }
        }
        public static bool IsMoveFileEnable => DestinationFolderPath != null;
        public static ObservableCollection<string> Filters => filters;
        public static bool IsSaveCreationTimeEnable
        {
            get
            {
                var setting = localSettings.Values[nameof(IsSaveCreationTimeEnable)];
                return setting == null ? false : (bool)setting;
            }
            set { localSettings.Values[nameof(IsSaveCreationTimeEnable)] = value; }
        }
        public static bool IsDateTreatingEnable
        {
            get
            {
                var setting = localSettings.Values[nameof(IsDateTreatingEnable)];
                return setting == null ? false : (bool)setting;
            }
            set { localSettings.Values[nameof(IsDateTreatingEnable)] = value; }
        }


        public static void Initialize()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            localFolder = ApplicationData.Current.LocalFolder;

            CreateOrReadFilters();
        }

        private async static void CreateOrReadFilters()
        {
            StorageFile storageFile = await localFolder.CreateFileAsync(FiltersFileName, CreationCollisionOption.OpenIfExists);
            var lines = await FileIO.ReadLinesAsync(storageFile);
            filters = new ObservableCollection<string>(lines);
            filters.CollectionChanged += Filters_CollectionChanged;
        }

        private async static void Filters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            filters.CollectionChanged -= Filters_CollectionChanged;
            filters.Sort();
            Filters.CollectionChanged += Filters_CollectionChanged;
            StorageFile filtersFile = await localFolder.GetFileAsync(FiltersFileName);
            await FileIO.WriteLinesAsync(filtersFile, filters);
        }

        private static void Sort<T>(this ObservableCollection<T> collection)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort();

            for (int i = 0; i < sortableList.Count; i++)
                collection.Move(collection.IndexOf(sortableList[i]), i);
        }
    }
}
