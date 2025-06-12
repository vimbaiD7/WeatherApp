namespace WeatherApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            VersionTracking.Track();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new Window();

            if (VersionTracking.IsFirstLaunchEver == true)
            {
                window.Page = new WelcomePage();
            }
            else
            {
                window.Page = new WeatherPage();
            }

            return window;
        }
    }
} 