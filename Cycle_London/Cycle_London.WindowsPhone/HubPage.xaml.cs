using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Cycle_London.Common;

namespace Cycle_London
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _viewModel = new ObservableDictionary();
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView(@"Resources");
        private MapControl _bikeMapControl = new MapControl();
        private int _ringCount;

        public HubPage()
        {
            InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            NavigationCacheMode = NavigationCacheMode.Required;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public ObservableDictionary ViewModel
        {
            get { return this._viewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //LOCALISATION
            MapHeader.Header = _resourceLoader.GetString(@"HUB_HEADER_FINDABIKE");
            ExtrasHeader.Header = _resourceLoader.GetString(@"HUB_HEADER_EXTRAS");
            AboutHeader.Header = _resourceLoader.GetString(@"HUB_HEADER_ABOUT");
            Bikeimage.Visibility = Visibility.Collapsed;


            //Get users locationa and set map
            var geolocator = new Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();
            if (geoposition != null)
            {
                _bikeMapControl.Center = geoposition.Coordinate.Point;
                _bikeMapControl.ZoomLevel = 13;
            }


        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void FlyoutTapEvents_OnTapped(object sender, RoutedEventArgs routedEventArgs)
        {
            var selectedflyoutItem = sender as AppBarButton;

            if (selectedflyoutItem != null)
            {
                switch (selectedflyoutItem.Tag.ToString())
                {
                    case "Programmr":
                        LaunchSite(new Uri("http://www.programmr.com"));
                        break;
                    case "Hired":
                        LaunchSite(new Uri("https://hired.com/?utm_source=programmr"));
                        break;
                    case "Icons":
                        LaunchSite(new Uri("http://modernuiicons.com/"));
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


        async void LaunchSite(Uri uri)
        {
            await Launcher.LaunchUriAsync(uri);
        }

        private void ListViewTapEvents_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedListItem = sender as ListViewItem;
            if (selectedListItem != null)
            {
                switch (selectedListItem.Tag.ToString())
                {
                    case "Costs":
                        Debug.WriteLine("{0} pressed", selectedListItem);
                        Frame.Navigate(typeof(CostsPage));
                        break;
                    case "Points":
                        Debug.WriteLine("{0} pressed", selectedListItem);
                        Frame.Navigate(typeof(BikePointsPage));
                        break;
                    case "Stats":
                        Debug.WriteLine("{0} pressed", selectedListItem);
                        Frame.Navigate(typeof(StatsPage));
                        break;
                }
            }
            else
            {
                throw new Exception("Flyout item has not been tagged or statment is spelt wrong.");
            }
        }


        private void BikeMap_OnLoaded(object sender, RoutedEventArgs e)
        {
            _bikeMapControl = sender as MapControl;
        }

        private async void FineMe_OnClick(object sender, RoutedEventArgs e)
        {
            var geolocator = new Geolocator();
            var geoposition = await geolocator.GetGeopositionAsync();
            if (geoposition != null)
            {
                _bikeMapControl.Center = geoposition.Coordinate.Point;
                _bikeMapControl.ZoomLevel = 15;
            }
        }
    }
}
