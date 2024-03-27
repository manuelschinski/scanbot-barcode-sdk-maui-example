// This file has been autogenerated from a class added in the UI designer.


using BarcodeSDK.NET.iOS.Utils;
using ScanbotSDK.iOS;

namespace BarcodeSDK.NET.iOS.Controllers.ClassicComponents.TableViewCells
{
    public partial class BarcodeScanAndCountResultCell : UITableViewCell
	{
		public BarcodeScanAndCountResultCell (IntPtr handle) : base (handle)
		{
		}

        public override void AwakeFromNib()
        {
            lblBarcodeResult.Lines = lblBarcodeType.Lines = 0;
            lblBarcodeCount.TextColor = UIColor.SystemBlue;
        }

        internal void PopulateData(SBSDKBarcodeScannerAccumulatingResult barcode)
		{
            lblBarcodeCount.Text = "x" + (int)barcode.ScanCount;
			lblBarcodeResult.Text = barcode.Code.RawTextString;
            lblBarcodeType.Text = barcode.Code.Type.Name;
            Utilities.CreateRoundedCardView(containerView);
        }
    }
}
