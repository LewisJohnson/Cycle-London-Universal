namespace Cycle_London.DataModels
{
    class DataSourceUrl
    {

        private static string _liveUpdates = "http://api.tfl.gov.uk/BikePoint?app_id=4238c1bf&app_key=eaf40b6c793133ac35d48fc1ec6eccdd";


        public static string LiveUpdatesUrl
        {
            get { return _liveUpdates; }
            set { _liveUpdates = value; }


        }
    }
}
