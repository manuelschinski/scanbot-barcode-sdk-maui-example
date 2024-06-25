namespace BarcodeScannerManuel.forms
{
	public partial class GoogleVision : ContentPage
	{
		public GoogleVision()
		{
			InitializeComponent();

			BindingContext = new ScangineViewModel();
		}
    }
}