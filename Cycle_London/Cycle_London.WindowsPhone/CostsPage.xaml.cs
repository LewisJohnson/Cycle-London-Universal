using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cycle_London.Common;

namespace Cycle_London
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class CostsPage : Page
    {

        private readonly NavigationHelper _navigationHelper;
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView(@"Resources");


        public CostsPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            //LOCALISATION
            try
            {

            CostsBlock.Text = _resourceLoader.GetString(@"GENERAL_COST");
            DescriptionBlock.Text = _resourceLoader.GetString(@"GENERAL_DESC");
                
            Price30MinsBlock.Text = _resourceLoader.GetString(@"COSTS_30MINS_TITLE");
            ThirtyMinsBlock.Text = _resourceLoader.GetString(@"COSTS_30MINS_DESC");

            Price24HBikeTextBox.Text = _resourceLoader.GetString(@"COSTS_24HOUR_TITLE");
            DayBikeTextBox.Text = _resourceLoader.GetString(@"COSTS_24HOUR_DESC");

            PriceReturnBlock.Text = _resourceLoader.GetString(@"COSTS_RETURN_TITLE");
            ReturnBikeBlock.Text = _resourceLoader.GetString(@"COSTS_RETURN_DESC");

            InformationTextBlock.Text = _resourceLoader.GetString(@"COSTS_INFORMATION_DESC");
            WarningTextBlock.Text = _resourceLoader.GetString(@"COSTS_WARNING_DESC");

            Journey1Block.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_1_TITLE");
            Journey1BlockInformation.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_1_DESC");

            Journey2Block.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_2_TITLE");
            Journey2BlockInformation.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_2_DESC");

            Journey3Block.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_3_TITLE");
            Journey3BlockInformation.Text = _resourceLoader.GetString(@"COSTS_JOURNEY_3_DESC");

            JourneysBlock.Text = _resourceLoader.GetString(@"COSTS_JOURNEYS_TITLE");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Environment.FailFast("Failed to get resouces." + ex);
            }
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
    }
}
