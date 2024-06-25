#if ANDROID
using Scandit.DataCapture.Barcode.UI.Overlay;
using BarcodeScannerManuel.Platforms.Android;
using Scandit.DataCapture.Core.UI.Viewfinder;
using Scandit.DataCapture.Core.Common.Geometry;
using Color = Android.Graphics.Color;

#nullable enable

namespace BarcodeScannerManuel.forms
{
    public partial class FullscreenScandit : ContentPage
    {
        private BarcodeCaptureOverlay overlay;

        public FullscreenScandit()
	    {
		    this.InitializeComponent();
		    BindingContext = App.Services.GetService<Android_FullscreenViewModel>();            

            this.dataCaptureView.HandlerChanged += DataCaptureViewHandlerChanged;
        }

        private void DataCaptureViewHandlerChanged(object? sender, EventArgs e)
        {
            this.overlay = BarcodeCaptureOverlay.Create(this.viewModel.BarcodeCapture, BarcodeCaptureOverlayStyle.Frame);
            LaserlineViewfinder viewfinder = new LaserlineViewfinder(LaserlineViewfinderStyle.Animated);
            var Unit = new FloatWithUnit(450, MeasureUnit.Pixel);
            viewfinder.Width = Unit;
            viewfinder.EnabledColor = Color.Green;
            viewfinder.DisabledColor = Color.Red;

            this.overlay.Viewfinder = viewfinder;

            this.dataCaptureView.AddOverlay(overlay);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = this.viewModel.OnResumeAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.viewModel.OnSleep();
        }
    }
}
#endif