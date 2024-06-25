using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BarcodeScannerManuel.forms
{
	public partial class MainPage : ContentPage
	{
        public MainPage()
		{
			InitializeComponent();

			BindingContext = App.Services.GetService<MainPageViewModel>();
		}
	}
}