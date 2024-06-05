using ScanbotSDK.MAUI.Configurations;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI.Models;


namespace ScanbotSDK.MAUI.Example.Pages;

public partial class BarcodeFinderClassicComponentPage : BaseComponentPage
{
    public BarcodeFinderClassicComponentPage()
    {
        InitializeComponent();

        cameraView.FinderConfiguration = new FinderConfiguration()
        {
            IsFinderEnabled = true,
            FinderLineColor = Colors.Red,
            FinderOverlayColor = Colors.Gray,
#if ANDROID
            FinderLineWidth = 10,
            FinderLineCornerRadius = 20,
            FinderMinimumPadding = 50,
#elif IOS
            FinderLineWidth = 2,
            FinderLineCornerRadius = 5,
            FinderMinimumPadding = 2,
#endif
        };
    }
        
    private void HandleScannerResults(BarcodeResultBundle result)
    {
        string text = string.Empty;

        if (result?.Barcodes != null)
        {
            foreach (Barcode barcode in result.Barcodes)
            {
                text += $"{barcode.Text} ({barcode.Format.ToString().ToUpper()})\n";
            }
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            System.Diagnostics.Debug.WriteLine(text);
            lblResult.Text = text;
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Start barcode detection manually
        cameraView.StartDetection();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop barcode detection manually
        cameraView.StopDetection();
    }

    private void CameraView_OnOnBarcodeScanResult(BarcodeResultBundle result)
    {
        HandleScannerResults(result);
    }
}