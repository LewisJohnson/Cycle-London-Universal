using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Cycle_London.Common;

namespace Cycle_London
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Properties and stuff

        private readonly NavigationHelper _navigationHelper;
        private static Frame _frame = new Frame();
        private Rectangle _appSelector = new Rectangle();
        private static Grid _mainGrid = new Grid();

        #endregion

        public MainPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        public static Grid MainGrid
        {
            get { return _mainGrid; }
            set { _mainGrid = value; }
        }

        public static Frame HostFrame
        {
            get { return _frame; }
            set { _frame = value; }
        }


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

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

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            _frame = sender as Frame;
            _frame.Navigate(typeof (BikeHubPage));
        }

        private void AppSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            _appSelector = sender as Rectangle;
        }

        private void MainPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            HostFrame.Width = Window.Current.Bounds.Width;
            _appSelector.Width = Window.Current.Bounds.Width;
            if (BikeHubPage.HubPageLoaded)
            {
                BikeHubPage.HubPage.Height = MainGrid.RowDefinitions[1].ActualHeight;
                BikeHubPage.HubPage_OnSizeChanged();
            }
            
        }

        private void MainGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainGrid = sender as Grid;
        }
    }

}



