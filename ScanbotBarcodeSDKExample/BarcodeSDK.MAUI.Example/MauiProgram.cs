using ScanbotSDK.MAUI.Models;

namespace ScanbotSDK.MAUI.Example
{
    public static class MauiProgram
    {
        // Without a license key, the Scanbot Barcode SDK will work for 1 minute.
        // To scan longer, register for a trial license key here: https://scanbot.io/trial/
        public const string LicenseKey = "gZ7vXNAYRllrUdmGI7haD+D1o02tc0" +
                                          "CZP9ufvgZU1C6FWIMaU2tfOtq/A0Nh" +
                                          "wmgOocKUw5/+PXiecYGkEK6Kure9ku" +
                                          "zEVSfOxonFtkKgNEItDZ7q2ix1hDi/" +
                                          "z8LNvsq+XRlt/Chss3MHOaLJd3IG0M" +
                                          "4EFD3vB3giZAe2UBcyKi10BGx4QydD" +
                                          "+dW0ad14ij/uYEWcz/P4Oj4ATldQIU" +
                                          "TTfdcwjHd/WlbxbbWJKYS5eqDdsOIP" +
                                          "4Xpi95mG7lm1xfeVHcGuGR3enQy4Ox" +
                                          "xvJwqVy5lAIQs7PawwJmNZu6vlUBe5" +
                                          "C0Be3TyiT4lSpEs3BBLxMEb2E6QDj4" +
                                          "lJcMGvpKfuFA==\nU2NhbmJvdFNESw" +
                                          "ppby5zY2FuYm90LmV4YW1wbGUuc2Rr" +
                                          "LmJhcmNvZGUubmV0CjE3MTk5NjQ3OT" +
                                          "kKODM4ODYwNwoxOQ==\n\"";
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            ScanbotSDKInitialize(builder);

            return builder.Build();
        }

        private static void ScanbotSDKInitialize(MauiAppBuilder mauiApp)
        {
            ScanbotBarcodeSDK.Initialize(mauiApp, new InitializationOptions
            {
                LicenseKey = LicenseKey,
                LoggingEnabled = true,
                ErrorHandler = (status, feature) =>
                {
                    Console.WriteLine($"License error: {status}, {feature}");
                }
            });
        }
    }
}