using BarcodeScannerManuel.objects;
using BarcodeScannerManuel.sys;
using Scandit.DataCapture.Core.UI.Viewfinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BarcodeScannerManuel.forms
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Variables
        private readonly ScannedItemService _scannedItemService;

        #endregion

        #region Properties
        private ObservableCollection<string> _SelectedViewfinderMode = new()
        {
            {"Normaler Scan" },
            {"Vollbild Scan" }
        };
        public ObservableCollection<string> SelectedViewfinderMode
        {
            get => _SelectedViewfinderMode;
            set
            {
                _SelectedViewfinderMode = value;
                NotifyPropertyChanged(nameof(SelectedViewfinderMode));
            }
        }

        private int _SelectedMode = 0;
        public int SelectedMode
        {
            get => _SelectedMode;
            set
            {
                _SelectedMode = value;
                
                NotifyPropertyChanged(nameof(SelectedMode));
            }
        }

        public ObservableCollection<ScannedItem> Scannedlist => _scannedItemService.ScannedItems;

        private ScannedItem _Scanneditem = new();
        public ScannedItem Scanneditem
        {
            get => _Scanneditem;
            set
            {
                _Scanneditem = value;
                NotifyPropertyChanged(nameof(Scanneditem));
            }
        }

        private string _ID = string.Empty;
        public string ID
        {
            get => _ID;
            set
            {
                foreach(ScannedItem item in Scannedlist)
                {
                    _ID = item.ID + 1 .ToString();
                }

                NotifyPropertyChanged(nameof(ID));
            }
        }

        private string _Barcodetyp = string.Empty;
        public string Barcodetyp
        {
            get => _Barcodetyp;
            set
            {
                foreach(ScannedItem item in Scannedlist)
                {
                    _Barcodetyp = item.Barcodetyp;
                }

                NotifyPropertyChanged(nameof(Barcodetyp));
            }
        }

        private string _Productcode = string.Empty;
        public string Productcode
        {
            get => _Productcode;
            set
            {
                foreach(ScannedItem item in Scannedlist)
                {
                    _Productcode = item.Productcode;
                }

                NotifyPropertyChanged(nameof(Productcode));
            }
        }
        #endregion
        #region Action 
        protected Command _ScanditCommand;
        public ICommand ScanditCommand { get { return _ScanditCommand; } }

        protected Command _ExtractCommand;
        public ICommand ExtractCommand { get { return _ExtractCommand; } }
        #endregion

        #region Methoden
        public MainPageViewModel() { }
        public MainPageViewModel(ScannedItemService scannedItemService)
        {
            DeviceDisplay.Current.KeepScreenOn = false;

            _scannedItemService = scannedItemService;
            _scannedItemService.ScannedItemsUpdated += OnScannedItemsUpdated;

            _ScanditCommand = new Command(OnScandit);
            _ExtractCommand = new Command(OnExtract);
        }

        /// <summary>
        /// Updatet die Scanned Item List
        /// </summary>
        private void OnScannedItemsUpdated()
        {
            NotifyPropertyChanged(nameof(Scannedlist));
        }

        /// <summary>
        /// Öffnet je nach gewählten Modus die passende Page
        /// </summary>
        private void OnScandit()
        {
            switch (SelectedMode)
            {
                case 0:
                    {
                        Shell.Current.GoToAsync("//NormalScan");
                        break;
                    }
                case 1:
                    {
                        Shell.Current.GoToAsync("//FullscreenScan");
                        break;
                    }
            }
        }

        /// <summary>
        /// Lädt die Aktuelle Liste der gescannten Barcodes herunter
        /// </summary>
        private void OnExtract()
        {

        }
        #endregion
    }
}
