using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Bing.Maps;
using Cycle_London.Common;
using Cycle_London.DataModels;

namespace Cycle_London
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage
    {
        #region Properties and stuff

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _viewModel = new ObservableDictionary();
        private readonly ObservableCollection<ObservableCollection<CollectedDataGroup>> _bikeDataCollection =
            new ObservableCollection<ObservableCollection<CollectedDataGroup>>();
        private ObservableCollection<ObservableCollection<CollectedDataGroup>> _cacheDataCollection =
            new ObservableCollection<ObservableCollection<CollectedDataGroup>>();
        private ListView _bikepointListView = new ListView();
        private Map _bikeMap = new Map();
        private ComboBox _searchBy = new ComboBox();
        private TextBlock _mainName = new TextBlock();
        private TextBlock _mainDocks = new TextBlock();
        private TextBlock _mainBikes = new TextBlock();
        private Canvas _mapInfoCanvas = new Canvas();
        private ScrollViewer _bikePointsScrollViewer = new ScrollViewer();
        private ScrollViewer _otherScrollViewer = new ScrollViewer();
        private ScrollViewer _mapScrollViewer = new ScrollViewer();
        private Canvas _mapCanvas = new Canvas();
        private Rectangle _infoRectangle = new Rectangle();
        private Grid _infoGrid = new Grid();

        private readonly Pushpin _selfpushpin = new Pushpin
        {
            Text = "!",
            Background = new SolidColorBrush(Colors.OrangeRed)

        };

        //-----------------------------------//

        private bool _descExpanded;
        private bool _tapped;
        private double _descwidth = 100;
        private double _descTextFaded = 0.3;
        private double _descTextFull = 1;
        private double _descTextLarge = 20;
        private double _descTextSmall = 11;
        private int _ringCount;

        //-----------------------------------//

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public ObservableDictionary ViewModel
        {
            get { return _viewModel; }
        }

        public ObservableCollection<ObservableCollection<CollectedDataGroup>> BikeDataCollection
        {
            get { return _bikeDataCollection; }
        }

        public ObservableCollection<ObservableCollection<CollectedDataGroup>> CacheDataCollection
        {
            get { return _cacheDataCollection; }
            set { _cacheDataCollection = value; }
        }

        public ObservableCollection<ObservableCollection<CollectedDataGroup>> SearchedBikeDataCollection
        {
            get { return _bikeDataCollection; }
        }

        public IEnumerable<DataGroup> BikeDataGroups { get; set; }


        #endregion

        public HubPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await GetData();
            Canvas.SetTop(_infoRectangle, _mapCanvas.Height - 120);
            Canvas.SetTop(_infoGrid, _mapCanvas.Height - 120);
        }

        private async Task GetData()
        {
            Bikeimage.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;
            TextBlock.Visibility = Visibility.Visible;
            Backdrop.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;

            var temp = new ObservableCollection<CollectedDataGroup>();
            var statsDataGroups = await StatsDataSource.GetGroupsAsync();
            BikeDataGroups = await BikePointDataSource.GetGroupsAsync();
            foreach (var item in BikeDataGroups)
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

                var pushpin = new Pushpin
                {
                    Background = new SolidColorBrush(Colors.DodgerBlue)
                }
                MapLayer.SetPosition(pushpin, new Location(item.Lat, item.Lon));
                _bikeMap.Children.Add(pushpin);
                pushpin.Tapped += BikePointPin_Tapped;
                pushpin.PointerEntered += BikePointPin_PointerEntered;
                pushpin.PointerExited += BikePointPin_PointerExited;
            }

            CacheDataCollection.Add(temp);
            BikeDataCollection.Add(temp);
            TextBlock.Text = "Adding pins to map.";

            //Set map location
            try
            {
                TextBlock.Text = "Getting your location.";
                var geolocator = new Geolocator();
                var geoposition = await geolocator.GetGeopositionAsync();
                if (geoposition != null)
                {
                    _bikeMap.SetView(new Location(geoposition.Coordinate.Point.Position.Latitude,
                        geoposition.Coordinate.Point.Position.Longitude), 18, TimeSpan.FromSeconds(3));

                    MapLayer.SetPosition(_selfpushpin, new Location(geoposition.Coordinate.Point.Position.Latitude,
                        geoposition.Coordinate.Point.Position.Longitude));
                    _bikeMap.Children.Remove(_selfpushpin);
                    _bikeMap.Children.Add(_selfpushpin);
                }
            }
            catch (Exception)
            {
                _bikeMap.SetView(new Location(51.5072, 0.1275), 15);
            }

            if (!BikePointDataSource.Failed)
            {
                ProgressRing.Visibility = Visibility.Collapsed;
                TextBlock.Visibility = Visibility.Collapsed;
                Backdrop.Visibility = Visibility.Collapsed;
                ProgressRing.IsActive = false;
                ViewModel["Costs"] = statsDataGroups;
                ViewModel["BikesShort"] = BikeDataCollection;

            }
            else
            {
                await GetLocalData();
                Debug.WriteLine("Using local data.");
                var fail = new MessageDialog("We failed to reach TFL's servers. Using local data, no real-time updates.",
                    "Something went wrong :(");
                fail.ShowAsync();
            }

        }

        private async Task GetLocalData()
        {
            Backdrop.Fill = new SolidColorBrush(Colors.DarkRed);
            TextBlock.Text = "Something went wrong...";
            Bikeimage.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;
            TextBlock.Visibility = Visibility.Visible;
            Backdrop.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;

            var temp = new ObservableCollection<CollectedDataGroup>();
            var statsDataGroups = await StatsDataSource.GetGroupsAsync();
            BikeDataGroups = await BikePointDataSource.GetLocalGroupsAsync();
            foreach (var item in BikeDataGroups)
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

                var pushpin = new Pushpin()
                {
                    Background = new SolidColorBrush(Colors.DodgerBlue),

                };
                MapLayer.SetPosition(pushpin, new Location(item.Lat, item.Lon));
                _bikeMap.Children.Add(pushpin);
                pushpin.Tapped += BikePointPin_Tapped;
                pushpin.PointerEntered += BikePointPin_PointerEntered;
                pushpin.PointerExited += BikePointPin_PointerExited;
            }

            CacheDataCollection.Add(temp);
            BikeDataCollection.Add(temp);

            //Set map location
            try
            {
                TextBlock.Text = "Getting your location.";
                var geolocator = new Geolocator();
                var geoposition = await geolocator.GetGeopositionAsync();
                if (geoposition != null)
                {
                    _bikeMap.SetView(new Location(geoposition.Coordinate.Point.Position.Latitude,
                        geoposition.Coordinate.Point.Position.Longitude), 18, TimeSpan.FromSeconds(3));

                    MapLayer.SetPosition(_selfpushpin, new Location(geoposition.Coordinate.Point.Position.Latitude,
                        geoposition.Coordinate.Point.Position.Longitude));
                    _bikeMap.Children.Remove(_selfpushpin);
                    _bikeMap.Children.Add(_selfpushpin);
                }
            }
            catch (Exception)
            {
                _bikeMap.SetView(new Location(51.5072, 0.1275), 15);
            }



            ProgressRing.Visibility = Visibility.Collapsed;
            TextBlock.Visibility = Visibility.Collapsed;
            Backdrop.Visibility = Visibility.Collapsed;
            ProgressRing.IsActive = false;
            ProgressRing.IsActive = false;


            ViewModel["Costs"] = statsDataGroups;
            ViewModel["BikesShort"] = BikeDataCollection;

        }

        #region PushPin Events

        private void BikePointPin_PointerExited(object sender, PointerRoutedEventArgs e)
        {

            _mapInfoCanvas.Opacity = 0.7;
            var pin = sender as Pushpin;
            pin.Background = new SolidColorBrush(Colors.DodgerBlue);

            if (!_tapped)
            {
                _mainName.Text = "";
                _mainDocks.Text = "";
                _mainBikes.Text = "";

            }

        }

        private void BikePointPin_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var up = new PlaneProjection { LocalOffsetY = 80 };
            _mapInfoCanvas.Projection = up;

            _mapInfoCanvas.Opacity = 0.7;
            _tapped = false;
            var pin = sender as Pushpin;
            pin.Background = new SolidColorBrush(Colors.Tomato);
            var pos = MapLayer.GetPosition(pin);

            foreach (var item in BikeDataGroups
                .Where(item => item.Lat == pos.Latitude)
                .Where(item => item.Lon == pos.Longitude))
            {
                _mainName.Text = item.CommonName;
            }

        }

        void BikePointPin_Tapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            var up = new PlaneProjection { LocalOffsetY = 0 };
            _mapInfoCanvas.Projection = up;
            _mapInfoCanvas.Opacity = 1;
            _tapped = true;
            var pin = sender as Pushpin;
            pin.Background = new SolidColorBrush(Colors.DodgerBlue);
            var pos = MapLayer.GetPosition(pin);



            _bikeMap.SetView(new Location(pos.Latitude, pos.Longitude));

            foreach (var item in BikeDataGroups
                .Where(item => item.Lat == pos.Latitude)
                .Where(item => item.Lon == pos.Longitude))
            {
                _mainName.Text = item.CommonName;
                _mainDocks.Text = item.AdditionalProperties[7].Value;
                _mainBikes.Text = item.AdditionalProperties[6].Value;
            }
        }

        #endregion

        private void SearchBox_OnQueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args)
        {
            var searchType = _searchBy.SelectedItem as ComboBoxItem;
            var searchQ = searchType.Content.ToString();
            if (args.QueryText.EndsWith(" "))
            {
                ViewModel["BikesShort"] = CacheDataCollection;
                return;
            }
            var temp = new ObservableCollection<CollectedDataGroup>();

            switch (searchQ)
            {
                case "Name":
                    if (args.QueryText.Length >= 3)
                    {
                        BikeDataCollection.Clear();
                        foreach (
                            var item in
                                BikeDataGroups.Where(
                                    item => item.CommonName.ToLower().Contains(args.QueryText.ToLower())))
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
                    }
                    else if (args.QueryText.Length == 0)
                    {
                        BikeDataCollection.Clear();
                        foreach (
                            var item in
                                BikeDataGroups)
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
                    }
                    else
                    {
                        return;
                    }

                    break;
                case "Id":
                    if (_bikepointListView.ItemTemplate != ExtendedBikePointTemplate)
                    {
                        _bikepointListView.ItemTemplate = ExtendedBikePointTemplate;
                    }
                    BikeDataCollection.Clear();

                    foreach (var item in BikeDataGroups.Where(item => item.Id.ToLower().Contains(args.QueryText.ToLower())))
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
                    break;
                case "Lat":
                    if (_bikepointListView.ItemTemplate != ExtendedBikePointTemplate)
                    {
                        _bikepointListView.ItemTemplate = ExtendedBikePointTemplate;
                    }
                    BikeDataCollection.Clear();
                    foreach (var item in BikeDataGroups.Where(item => item.Lat.ToString().ToLower().Contains(args.QueryText.ToLower())))
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
                    break;
                case "Lon":
                    if (_bikepointListView.ItemTemplate != ExtendedBikePointTemplate)
                    {
                        _bikepointListView.ItemTemplate = ExtendedBikePointTemplate;
                    }
                    BikeDataCollection.Clear();
                    foreach (var item in BikeDataGroups.Where(item => item.Lon.ToString().ToLower().Contains(args.QueryText.ToLower())))
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
                    break;
            }

            BikeDataCollection.Add(temp);
            ViewModel["BikesShort"] = BikeDataCollection;
        }

        #region Tapped Events

        private async void FlyoutTapEvents_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedflyoutItem = sender as MenuFlyoutItem;
            if (selectedflyoutItem != null)
            {
                switch (selectedflyoutItem.Tag.ToString())
                {
                    case "Programmr":
                        await Launcher.LaunchUriAsync(new Uri("http://www.programmr.com"));
                        break;
                    case "Hired":
                        await Launcher.LaunchUriAsync(new Uri("https://hired.com/?utm_source=programmr"));
                        break;
                    case "Icons":
                        await Launcher.LaunchUriAsync(new Uri("http://modernuiicons.com/"));
                        break;
                }

            }
            else
            {
                throw new Exception("Flyout item has not been tagged or statment is spelt wrong.");
            }
        }

        private void RingRing_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ++_ringCount;
            if (_ringCount != 5) return;
            Bikeimage.Visibility = Visibility.Visible;
            RingRingBikeStoryboard.Begin();
            RingRing.Play();
            _ringCount = 0;


        }

        private void DiscGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var textblock1 = (TextBlock)grid.Children[0];
                var textblock2 = (TextBlock)grid.Children[1];
                var textblock3 = (TextBlock)grid.Children[2];
                var gridChild = (Grid)grid.Children[3];

                var textblockChild1 = (TextBlock)gridChild.Children[0];
                var picblockChild1 = (Image)gridChild.Children[1];
                var textblockChild2 = (TextBlock)gridChild.Children[2];

                if (_descExpanded != true)
                {
                    grid.Width = 512;
                    textblock1.TextAlignment = TextAlignment.Justify;

                    textblock1.FontSize = _descTextLarge;
                    textblock2.FontSize = _descTextLarge;
                    textblock3.FontSize = _descTextLarge;

                    picblockChild1.Opacity = _descTextFull;
                    textblockChild1.Opacity = _descTextFull;
                    textblockChild2.Opacity = _descTextFull;
                    textblock1.Opacity = _descTextFull;
                    textblock2.Opacity = _descTextFull;
                    textblock3.Opacity = _descTextFull;

                    _descExpanded = true;
                }
                else
                {
                    grid.Width = _descwidth;
                    textblock1.TextAlignment = TextAlignment.Center;

                    textblock1.FontSize = _descTextSmall;
                    textblock2.FontSize = _descTextSmall;
                    textblock3.FontSize = _descTextSmall;

                    picblockChild1.Opacity = _descTextFaded;
                    textblockChild1.Opacity = _descTextFaded;
                    textblockChild2.Opacity = _descTextFaded;
                    textblock1.Opacity = _descTextFaded;
                    textblock2.Opacity = _descTextFaded;
                    textblock3.Opacity = _descTextFaded;

                    _descExpanded = false;
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarToggleButton;
            if (btn == null) return;
            var check = btn.IsChecked;
            _bikepointListView.ItemTemplate = check != null && check.Value ? ExtendedBikePointTemplate : StandardBikePointTemplate;
        }


        private async void FindMe_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                var geolocator = new Geolocator();
                var geoposition = await geolocator.GetGeopositionAsync();
                if (geoposition != null)
                {
                    _bikeMap.SetView(new Location(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude), 18);
                    MapLayer.SetPosition(_selfpushpin, new Location(geoposition.Coordinate.Point.Position.Latitude,
                        geoposition.Coordinate.Point.Position.Longitude));
                    _bikeMap.Children.Remove(_selfpushpin);
                    _bikeMap.Children.Add(_selfpushpin);

                    GlobalHub.ScrollToSection(MapSection);
                }
            }
            catch (Exception)
            {
                _bikeMap.SetView(new Location(51.5072, 0.1275), 15);
                GlobalHub.ScrollToSection(MapSection);
            }
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (CollectedDataGroup)e.ClickedItem;
            _bikeMap.SetView(new Location(item.Lat, item.Lon), 17);
            _mainName.Text = item.CommonName;
            _mainDocks.Text = item.NbEmptyDocks;
            _mainBikes.Text = item.NbBikes;
        }

        #endregion

        #region OnLoaded Events
        // ReSharper disable PossibleNullReferenceException
        private void BikeSearchCombo_OnLoaded(object sender, RoutedEventArgs e)
        {
            _searchBy = sender as ComboBox;
        }

        private void ScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            _bikePointsScrollViewer = sender as ScrollViewer;
            _bikePointsScrollViewer.Height = Window.Current.Bounds.Height - 300;
        }

        private void BikeListLoaded(object sender, RoutedEventArgs e)
        {
            _bikepointListView = sender as ListView;
            if (_bikepointListView != null)
                _bikepointListView.ItemTemplate = StandardBikePointTemplate;

        }

        private void BikeMap_OnLoaded(object sender, RoutedEventArgs e)
        {
            _bikeMap = sender as Map;
        }

        private void MainBikes_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainBikes = sender as TextBlock;
        }

        private void MainDocks_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainDocks = sender as TextBlock;
        }

        private void MainName_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainName = sender as TextBlock;
        }

        private void MapInfo_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mapInfoCanvas = sender as Canvas;
        }
        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            _otherScrollViewer = sender as ScrollViewer;
            _otherScrollViewer.Height = Window.Current.Bounds.Height - 250;
        }
        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mapScrollViewer = sender as ScrollViewer;

            _mapScrollViewer.Height = Window.Current.Bounds.Height - 250;

        }
        private void Mapcanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mapCanvas = sender as Canvas;
            _mapCanvas.Height = Window.Current.Bounds.Height - 250;
        }

        private void InfoRectangle_OnLoaded(object sender, RoutedEventArgs e)
        {
            _infoRectangle = sender as Rectangle;
        }

        private void InfoGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            _infoGrid = sender as Grid;
        }
        // ReSharper restore PossibleNullReferenceException
        #endregion

        private void HubPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _bikePointsScrollViewer.Height = Window.Current.Bounds.Height - 300;
            _otherScrollViewer.Height = Window.Current.Bounds.Height - 250;
            _mapScrollViewer.Height = Window.Current.Bounds.Height - 250;
            _mapCanvas.Height = Window.Current.Bounds.Height - 250;
            Canvas.SetTop(_infoRectangle, _mapCanvas.Height - 120);
            Canvas.SetTop(_infoGrid, _mapCanvas.Height - 120);
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


    }

}



