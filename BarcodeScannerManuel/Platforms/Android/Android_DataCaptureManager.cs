using Android.Health.Connect.DataTypes;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Selection.Capture;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Gestures;

namespace BarcodeScannerManuel.Platforms.Android
{
    public class Android_DataCaptureManager
    {
        public const string ANDROID_SCANDIT_LICENSE_KEY = "AVwuf4iBHE5gFCB1uhW4ICsgS+avHmyrJm+EKaZzoPtaeZIT8X8XdbB7VbitWyF7w2HfKVhEmU7ASnc/WTFMozZbUTqWXRtW2G41uYhgoE33WFvuhXaZZ/Il8qx6HBpUgC+gxqt0c/k4qQnScTSh0d1Tm0Vg/av5C0t+pBj8DkbN7UA7Ldg8mYWy8wFxyvT55qmOUpY1dGzR8zki3THCeUTcojlybnM1zk3otI+nql11U84ZKN+SYkadKV1Rs/ij0TPSULC8vrPIUejPrERyTLN39PLANeDXE7tXQi5UOFeEqfNK68g9WzPiVl7LgBDv7LV8WAvCkv+AvqBNRSuny3G0m+UT6tIuTTSe/Xn/v3flwxtMSATbmjgEMUkU1P+o8V14OaMjWTocjnTNJFcOI6hn+0d+rmKpaI1s6HZ3Q0zSX7PSjqlBie/Qm4ajaIi4F7PEi4ullgkb3MTS8lPVOQliWsZNg3reKfoy7oXirWm95Bq5WcdyqWZ1aNawn+ozBzIibhMdpbVji9CIhccTcF7p+qiWZlSk3PoXjgiQf8mgHiNy3dI9Ud/e+vmSIeYoCFu+1KDJhAe+TFC1Ivqs3Ps0w5X3GnbNgSdOWQ0usvGR/yXu8VXAvMqa84VoSU2djGBVJGEH/2Sfd6u7pzqGha6z2RFTfinj5ygHOyfUrT6CQ/4sRaVonzVOwAec6m6o6XSCz/0z/nYbxezbqnl+9aoOINKeMDykHbFdbZ8BqTTzXL+ckZA/lXGKmTFzGoXyItbiQqBFH5LfHohUlQJ23+2K0D2UjrbmlDzW2ixbdjw0K18=";
        
        private static readonly Lazy<Android_DataCaptureManager> instance = new(() => new Android_DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public static Android_DataCaptureManager Instance => instance.Value;

        /// <summary>
        /// Allgemeine Einstellungen für den Barcode Scanner
        /// </summary>
        private Android_DataCaptureManager()
        {
            
            CameraSettings.ShouldPreferSmoothAutoFocus = true;
            CameraSettings.FocusGestureStrategy = FocusGestureStrategy.AutoOnLocation;
            CurrentCamera?.ApplySettingsAsync(CameraSettings);

            // Create data capture context using your license key and set the camera as the frame source.
            DataCaptureContext = DataCaptureContext.ForLicenseKey(ANDROID_SCANDIT_LICENSE_KEY);
            DataCaptureContext.SetFrameSourceAsync(CurrentCamera);
            

            // The barcode capturing process is configured through barcode capture settings
            // which are then applied to the barcode capture instance that manages barcode recognition.
            BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Gs1Databar,
                Symbology.Qr,
                Symbology.DataMatrix,
                Symbology.Code39,
                Symbology.Code128,
            };

            BarcodeCaptureSettings.EnableSymbologies(symbologies);

            TimeSpan negativeTimeSpan = new(-1, -2, -3);
            BarcodeCaptureSettings.CodeDuplicateFilter = negativeTimeSpan;

            SymbologySettings symbolSettings;
            foreach(var symbol in BarcodeCaptureSettings.EnabledSymbologies)
            {
                symbolSettings = BarcodeCaptureSettings.GetSymbologySettings(symbol);
                symbolSettings.ColorInvertedEnabled = true;
            }

            BarcodeCapture = BarcodeCapture.Create(DataCaptureContext, BarcodeCaptureSettings);
        }

        #region DataCaptureContext
        public DataCaptureContext DataCaptureContext { get; private set; }
        #endregion

        #region CameraSettings
        public Camera? CurrentCamera { get; private set; } = Camera.GetCamera(CameraPosition.WorldFacing);
        public CameraSettings CameraSettings { get; set; } = BarcodeCapture.RecommendedCameraSettings;
        #endregion

        #region BarcodeCapture
        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }
        #endregion
    }
}
