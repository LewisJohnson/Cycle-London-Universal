using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Cycle_London.Common;
using Cycle_London.DataModels;

namespace Cycle_London
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class BikePointsPage : Page
    {

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _viewModel = new ObservableDictionary();

        private readonly ObservableCollection<ObservableCollection<CollectedDataGroup>> _bikeDataCollection =
            new ObservableCollection<ObservableCollection<CollectedDataGroup>>();

        public BikePointsPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public ObservableDictionary ViewModel
        {
            get { return this._viewModel; }
        }

        public ObservableCollection<ObservableCollection<CollectedDataGroup>> BikeDataCollection
        {
            get { return this._bikeDataCollection; }

        }



        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Header.Text = "Loading";
            var temp = new ObservableCollection<CollectedDataGroup>();
            var bikeDataGroups = await BikePointDataSource.GetGroupsAsync();

            foreach (var item in bikeDataGroups)
            {
                temp.Add(new CollectedDataGroup(
                    item.CommonName,
                    item.Id,
                    item.Lat,
                    item.Lon,
                    item.Url,
                    item.AdditionalProperties[0].Value,
                    item.AdditionalProperties[1].Value,
                    item.AdditionalProperties[2].Value,
                    item.AdditionalProperties[3].Value,
                    item.AdditionalProperties[4].Value,
                    item.AdditionalProperties[5].Value,
                    item.AdditionalProperties[6].Value,
                    item.AdditionalProperties[7].Value,
                    item.AdditionalProperties[8].Value));

            }

            BikeDataCollection.Add(temp);
            ViewModel["BikesShort"] = BikeDataCollection;

            BikeListView.ItemTemplate = StandardBikePointTemplate;

            Header.Text = "Bike Points";
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarToggleButton;
            if (btn == null) return;
            var check = btn.IsChecked;
            BikeListView.ItemTemplate = check != null && check.Value ? ExtendedBikePointTemplate : StandardBikePointTemplate;
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            Header.Text = "Loading";
            var temp = new ObservableCollection<CollectedDataGroup>();
            var bikeDataGroups = await BikePointDataSource.GetGroupsAsync();

            foreach (var item in bikeDataGroups)
            {
                temp.Add(new CollectedDataGroup(
                    item.CommonName,
                    item.Id,
                    item.Lat,
                    item.Lon,
                    item.Url,
                    item.AdditionalProperties[0].Value,
                    item.AdditionalProperties[1].Value,
                    item.AdditionalProperties[2].Value,
                    item.AdditionalProperties[3].Value,
                    item.AdditionalProperties[4].Value,
                    item.AdditionalProperties[5].Value,
                    item.AdditionalProperties[6].Value,
                    item.AdditionalProperties[7].Value,
                    item.AdditionalProperties[8].Value));

            }

            BikeDataCollection.Add(temp);
            ViewModel["BikesShort"] = BikeDataCollection;

            BikeListView.ItemTemplate = StandardBikePointTemplate;

            Header.Text = "Bike Points";
        }
    }
}
