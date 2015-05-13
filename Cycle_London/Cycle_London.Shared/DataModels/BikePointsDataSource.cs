using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Cycle_London.DataModels
{
    /// <summary>
    /// BikePoints item data model.
    /// </summary>
    public class AdditionalProperty
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public AdditionalProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// BikePoints group data model.
    /// </summary>
    public class DataGroup
    {

        public string Id { get; private set; }
        public string Url { get; private set; }
        public string CommonName { get; private set; }
        public double Lat { get; private set; }
        public double Lon { get; private set; }

        public ObservableCollection<AdditionalProperty> AdditionalProperties { get; private set; }

        public DataGroup(string id, string url, string commonName, double lat, double lon)
        {
            Id = id;
            Url = url;
            CommonName = commonName;
            Lat = lat;
            Lon = lon;

            AdditionalProperties = new ObservableCollection<AdditionalProperty>();
        }

    }

    public class CollectedDataGroup
    {

        public string Id { get; private set; }
        public string Url { get; private set; }
        public string TerminalName { get; set; }
        public string Installed { get; set; }
        public string Locked { get; set; }
        public string InstallDate { get; set; }
        public string RemovalDate { get; set; }
        public string Temporary { get; set; }
        public string CommonName { get; private set; }
        public double Lat { get; private set; }
        public double Lon { get; private set; }
        public string NbBikes { get; set; }
        public string NbEmptyDocks { get; set; }
        public string NbDocks { get; set; }

        public CollectedDataGroup(string commonName, string id, double lat, double lon, string url, string terminalName,
            string installed, string locked, string installDate, string removalDate, string temporary, string nbBikes,
            string nbEmptyDocks, string nbDocks)
        {
            Id = id;
            Url = url;
            TerminalName = terminalName;
            Installed = installed;
            Locked = locked;
            InstallDate = installDate;
            RemovalDate = removalDate;
            Temporary = temporary;
            CommonName = commonName;
            Lat = lat;
            Lon = lon;
            NbBikes = nbBikes;
            NbEmptyDocks = nbEmptyDocks;
            NbDocks = nbDocks;
        }

    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// BikePointDataSource initializes with data read from a static json file from TFL api. 
    /// </summary>
    public class BikePointDataSource
    {
        private static readonly BikePointDataSource _bikePointDataSource = new BikePointDataSource();
        private readonly ObservableCollection<DataGroup> _groups = new ObservableCollection<DataGroup>();


        public ObservableCollection<DataGroup> Groups
        {
            get { return _groups; }
        }

        public static bool Failed { get; private set; }


        public static async Task<IEnumerable<DataGroup>> GetGroupsAsync()
        {
            await _bikePointDataSource.GetDataAsync();
            return _bikePointDataSource.Groups;
        }

        public static async Task<IEnumerable<DataGroup>> GetLocalGroupsAsync()
        {
            await _bikePointDataSource.GetLocalDataAsync();
            return _bikePointDataSource.Groups;
        }

        public static async Task<DataGroup> GetGroupAsync(string uniqueId)
        {
            await _bikePointDataSource.GetDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _bikePointDataSource.Groups.Where((group) => group.CommonName.Equals(uniqueId));
            return matches.Count() == 1 ? matches.First() : null;
        }


        private async Task GetDataAsync()
        {
            var cts = new CancellationTokenSource();


            if (_groups.Count != 0)
                return;

            string data = null;

            try
            {
                cts.CancelAfter(10000);
                var request = new HttpRequestMessage(HttpMethod.Get, DataSourceUrl.LiveUpdatesUrl);
                var response = await new HttpClient().SendAsync(request, cts.Token);
                data = await response.Content.ReadAsStringAsync();
            }

            catch (Exception)
            {
                Failed = true;
            }
            if (data != null)
            {
                cts.CancelAfter(10000);
                try
                {
                    cts.CancelAfter(10000);
                    var outerArray = JsonValue.Parse(data);
                    var mainArray = outerArray.GetArray();
                    var goodArray = mainArray.GetArray();

                    foreach (var groupValue in goodArray)
                    {
                        var groupObject = groupValue.GetObject();
                        var group = new DataGroup(
                            groupObject["id"].GetString(),
                            groupObject["url"].GetString(),
                            groupObject["commonName"].GetString(),
                            groupObject.GetNamedNumber("lat"),
                            groupObject.GetNamedNumber("lon"));

                        foreach (var itemObject in
                            from JsonValue itemValue in groupObject["additionalProperties"].GetArray()
                            select itemValue.GetObject())
                        {
                            @group.AdditionalProperties.Add(new AdditionalProperty(
                                itemObject["key"].GetString(),
                                itemObject["value"].GetString()));
                        }

                        Groups.Add(@group);
                    }
                }
                catch (OperationCanceledException)
                {
                    Failed = true;
                }
            }
            else
            {
                Failed = true;
            }
        }

        private async Task GetLocalDataAsync()
        {
            if (_groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModels/BikePoint.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string data = await FileIO.ReadTextAsync(file);
            var outerArray = JsonValue.Parse(data);
            var mainArray = outerArray.GetArray();
            var goodArray = mainArray.GetArray();

            foreach (var groupValue in goodArray)
            {
                var groupObject = groupValue.GetObject();
                var group = new DataGroup(
                    groupObject["id"].GetString(),
                    groupObject["url"].GetString(),
                    groupObject["commonName"].GetString(),
                    groupObject.GetNamedNumber("lat"),
                    groupObject.GetNamedNumber("lon"));

                foreach (var itemObject in
                    from JsonValue itemValue in groupObject["additionalProperties"].GetArray()
                    select itemValue.GetObject())
                {
                    @group.AdditionalProperties.Add(new AdditionalProperty(
                        itemObject["key"].GetString(),
                        itemObject["value"].GetString()));
                }

                Groups.Add(@group);
            }

        }
    }
}