#if ANDROID
using BarcodeScannerManuel.forms;
using BarcodeScannerManuel.Platforms.Android;
using BarcodeScannerManuel.sys;
using Microsoft.Extensions.Logging;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Models;
using Scandit.DataCapture.Core.UI.Maui;

namespace BarcodeScannerManuel
{
    public static class MauiProgram
    {
        private const string ScanbotLicens = "abcLizenz";
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(h =>
                {
                    h.AddHandler(typeof(DataCaptureView), typeof(DataCaptureViewHandler));
                });
            builder.Services.AddSingleton<ScannedItemService>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddTransient<Android_ScanditViewModel>();
            builder.Services.AddTransient<Android_FullscreenViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            ScanbotSDKInitialize(builder);

            return builder.Build();
        }

        private static void ScanbotSDKInitialize(MauiAppBuilder mauiApp)
        {
            /*ScanbotBarcodeSDK.Initialize(mauiApp, new InitializationOptions
            {
                LicenseKey = ScanbotLicens,
                LoggingEnabled = true,
                ErrorHandler = (status, feature) =>
                {
                    Exception ex = new Exception($"License error: {status}, {feature}");
                }
            });*/
        }
    }
}
#endif
