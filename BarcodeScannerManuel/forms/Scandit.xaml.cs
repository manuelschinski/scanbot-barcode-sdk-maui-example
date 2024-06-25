#if ANDROID
using Scandit.DataCapture.Barcode.UI.Overlay;
using BarcodeScannerManuel.Platforms.Android;
using Scandit.DataCapture.Core.UI.Viewfinder;
using Android.Graphics;
using Color = Android.Graphics.Color;
using Scandit.DataCapture.Core.Common.Geometry;

#nullable enable

namespace BarcodeScannerManuel.forms
{
	public partial class Scandit : ContentPage
	{
		private BarcodeCaptureOverlay overlay;

        public Scandit()
		{
			this.InitializeComponent();

			BindingContext = App.Services.GetService<Android_ScanditViewModel>();

			this.dataCaptureView.HandlerChanged += DataCaptureViewHandlerChanged;
		}

        private void DataCaptureViewHandlerChanged(object? sender, EventArgs e)
        {
            this.overlay = BarcodeCaptureOverlay.Create(this.viewModel.BarcodeCapture, BarcodeCaptureOverlayStyle.Legacy);
            RectangularViewfinder viewfinder = new RectangularViewfinder(RectangularViewfinderStyle.Rounded, RectangularViewfinderLineStyle.Light);
            viewfinder.Color = Color.Green;
            viewfinder.DisabledColor = Color.Red;
            var floatUnit = new FloatWithUnit(300,MeasureUnit.Pixel);
            var size = new SizeWithUnit(floatUnit, floatUnit);
            viewfinder.SetSize(size);
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