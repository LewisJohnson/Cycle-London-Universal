using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;


namespace Cycle_London.DataModels
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class StatsDataItem
    {
        public StatsDataItem(string year, string hiredBikes)
        {
            Year = year;
            HiredBikes = hiredBikes;

        }

        public string Year { get; private set; }
        public string HiredBikes { get; private set; }

    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class StatsDataGroup
    {
        public StatsDataGroup(string uniqueId)
        {
            UniqueId = uniqueId;
            Items = new ObservableCollection<StatsDataItem>();
        }

        public string UniqueId { get; private set; }
        public ObservableCollection<StatsDataItem> Items { get; private set; }

    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class StatsDataSource
    {
        private static StatsDataSource _statsDataSource = new StatsDataSource();

        private ObservableCollection<StatsDataGroup> _groups = new ObservableCollection<StatsDataGroup>();


        public ObservableCollection<StatsDataGroup> Groups
        {
            get { return _groups; }
        }

        public static async Task<IEnumerable<StatsDataGroup>> GetGroupsAsync()
        {
            await _statsDataSource.GetStatsDataAsync();
            return _statsDataSource.Groups;
        }

        private async Task GetStatsDataAsync()
        {
            if (_groups.Count != 0)
                return;

            var dataUri = new Uri("ms-appx:///DataModels/StatsData.json");

            var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            var jsonText = await FileIO.ReadTextAsync(file);
            var jsonObject = JsonObject.Parse(jsonText);
            var jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                var groupObject = groupValue.GetObject();
                var group = new StatsDataGroup(
                    groupObject["UniqueId"].GetString());

                foreach (var itemObject in from JsonValue itemValue in groupObject["Items"].GetArray() select itemValue.GetObject())
                {
                    group.Items.Add(new StatsDataItem(
                        itemObject["Year"].GetString(),
                        itemObject["HiredBikes"].GetString()));
                }
                Groups.Add(group);
            }
        }
    }
}