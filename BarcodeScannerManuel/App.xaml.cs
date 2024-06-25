namespace BarcodeScannerManuel
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = new AppShell();
            Services = serviceProvider;
        }
    }
}
