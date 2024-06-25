using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Gestures;

namespace BarcodeScannerManuel.Platforms.iOS
{
    public class iOS_DataCaptureManager
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string IOS_SCANDIT_LICENSE_KEY = "AZ2eEbqBLmoGCmYsnRx/rzEMQWGoPYtU0EpCA+BGBj6nd4/xcVGH+aZGo9NEfw5MUWezN1JidGQNS1y/QGcJcK1cLyjybC+vLWpAPLsgo2NYemRq5mMCDK4qzvQpIon2PEMMxLZDbBkhJKTe3AQ+bS+siZ8qDWLwHaDrG9dHJFB6NPBkLAVMizW7NyBQ/W+KmPbhDNiXHyZsL+lYCaKkbquNcC69UsDoeJzjqZ0t9eBySWg0+PrrOfMeM8K4sy/jp3BJNalIYkvn12T630/mN7Hg2BioxSQegggYNoO3zGnUFProrfymPdZNRQiR3UwylWjl6BzW7I6gQf77fY0j5+HpQ5X2zkWoUCFVhp2PKxDj1OxFixkNqOhTpwU2YzOKjczMmZmLIjeuH9v8ZAOURWNls2wDefeDXiamx1eIlF7J+psYK4/fNp+OeqO1hotZHgXQ+4iHKJYO7LULiXX5m4mpm+lsxB1uwZpVKDtsgXCTQobSeacOrzDSzBaW0XYWjaThSvFXes0UgQfBXL7caDBP4+RtZ1ReNhvNYRxsciaPPZAyVwdCn45LpAXBYLkDjeZw76PPIMet1OOUiaQDluoajbG0+rTMSBNo4P4mT6sebIvxHK1RCv0KzdUH1+q5BK5eLRayOvf3Vza9ALyuBxTdkOEXnOvNXDFfSN9CfBUd7IX/d27ycxIl7OlLpJgZXkIkYq4o63u6zUqRI95T11Q0A5IjVy6SvWuYPkWSZdGAE3TR1DO4EUiQ83t1ZN5IMgxRbubGapg+pocCUkQpFagiSjDMY75Hb5W0UkzIZ2LoLc3GJlI=";
        
        private static readonly Lazy<iOS_DataCaptureManager> instance = new Lazy<iOS_DataCaptureManager>(() => new iOS_DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public static iOS_DataCaptureManager Instance => instance.Value;

        private iOS_DataCaptureManager()
        {
            CameraSettings.ShouldPreferSmoothAutoFocus = true;
            CameraSettings.FocusGestureStrategy = FocusGestureStrategy.AutoOnLocation;
            CurrentCamera?.ApplySettingsAsync(CameraSettings);

            // Create data capture context using your license key and set the camera as the frame source.
            DataCaptureContext = DataCaptureContext.ForLicenseKey(IOS_SCANDIT_LICENSE_KEY);
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

            SymbologySettings dataMatrixSettings = BarcodeCaptureSettings.GetSymbologySettings(Symbology.DataMatrix);
            dataMatrixSettings.ColorInvertedEnabled = true;

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
