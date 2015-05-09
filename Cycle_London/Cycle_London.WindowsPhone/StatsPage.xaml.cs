using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cycle_London.Common;
using Cycle_London.DataModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Cycle_London
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatsPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;


        public StatsPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Header.Text = "Loading.";
            var statsDataGroups = await StatsDataSource.GetGroupsAsync();
            DefaultViewModel["Groups"] = statsDataGroups;
            Header.Text = "Statistics";
        }

    }
}
