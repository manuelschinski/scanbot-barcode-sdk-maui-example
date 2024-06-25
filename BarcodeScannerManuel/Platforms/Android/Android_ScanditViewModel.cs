using Android.Graphics;
using BarcodeScannerManuel.forms;
using BarcodeScannerManuel.objects;
using BarcodeScannerManuel.sys;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Count.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Viewfinder;
using System.Windows.Input;
using CameraPosition = Scandit.DataCapture.Core.Source.CameraPosition;
using Color = Android.Graphics.Color;
using FocusGestureStrategy = Scandit.DataCapture.Core.Source.FocusGestureStrategy;
using FrameSourceState = Scandit.DataCapture.Core.Source.FrameSourceState;
using ScanditCamera = Scandit.DataCapture.Core.Source.Camera;
using Size = Scandit.DataCapture.Core.Common.Geometry.Size;
using TorchState = Scandit.DataCapture.Core.Source.TorchState;
using Vibration = Scandit.DataCapture.Core.Common.Feedback.Vibration;

namespace BarcodeScannerManuel.Platforms.Android
{
    internal class Android_ScanditViewModel : BaseViewModel
    {

        #region Variablen      
        private readonly ScannedItemService _scannedItemService;
        internal ScannedItem ScannedItem;

        private IViewfinder viewfinder;
        internal IViewfinder ViewfinderStyle = new RectangularViewfinder(RectangularViewfinderStyle.Square, RectangularViewfinderLineStyle.Light);

        public ScanditCamera? Camera { get; private set; } = Android_DataCaptureManager.Instance.CurrentCamera;
        public DataCaptureContext DataCaptureContext { get; } = Android_DataCaptureManager.Instance.DataCaptureContext;
        public BarcodeCapture BarcodeCapture { get; } = Android_DataCaptureManager.Instance.BarcodeCapture;
        public Feedback Feedback { get; } = Feedback.DefaultFeedback;


        #endregion

        #region Properties

        public IViewfinder Viewfinder
        {
            get { return this.viewfinder; }
            set
            {
                this.viewfinder = value;
                this.NotifyPropertyChanged(nameof(Viewfinder));
            }
        }

        private string _SelectedMode = "Einzelscan";
        public string SelectedMode
        {
            get => _SelectedMode;
            set
            {
                _SelectedMode = value;
                NotifyPropertyChanged(nameof(SelectedMode));
            }
        }

        private bool _SwitchMode = false;
        public bool SwitchMode
        {
            get => _SwitchMode;
            set
            {
                _SwitchMode = value;
                if (value)
                {
                    SelectedMode = "Mehrfachscan";
                    IsScanButtonEnabled = false;
                }
                else
                {
                    SelectedMode = "Einzelscan";
                    IsScanButtonEnabled = true;
                }
                NotifyPropertyChanged(nameof(SwitchMode));
            }
        }

        private bool _IsScanButtonEnabled = true;
        public bool IsScanButtonEnabled
        {
            get => _IsScanButtonEnabled;
            set
            {
                _IsScanButtonEnabled = value;
                NotifyPropertyChanged(nameof(IsScanButtonEnabled));
            }
        }

        private float _SliderValue = 0;
        public float SliderValue
        {
            get => _SliderValue;
            set
            {
                if (value != _SliderValue)
                {
                    _SliderValue = (float)Math.Round(value, 1);
                    _SliderString = _SliderValue.ToString();

                    Zoom();
                }
                NotifyPropertyChanged(nameof(SliderValue));
                NotifyPropertyChanged(nameof(SliderString));
            }
        }

        private string _SliderString = "1";
        public string SliderString
        {
            get => _SliderString;
            set
            {
                if (_SliderString != _SliderValue.ToString())
                {
                    _SliderString = SliderValue.ToString();
                }
                NotifyPropertyChanged(nameof(SliderString));
            }
        }

        private IMediaPicker _CameraSource = MediaPicker.Default;
        public IMediaPicker CameraSource
        {
            get => _CameraSource;
            set
            {
                if (value != _CameraSource)
                {
                    _CameraSource = value;
                }
                NotifyPropertyChanged(nameof(CameraSource));
            }
        }

        private string _BarcodeSource = string.Empty;
        public string BarcodeSource
        {
            get => _BarcodeSource;
            set
            {
                if (value != _BarcodeSource)
                {
                    _BarcodeSource = value;
                }
                NotifyPropertyChanged(nameof(BarcodeSource));
            }
        }
        #endregion
        #region Action
        protected Command _ReturnCommand;
        public ICommand ReturnCommand { get { return _ReturnCommand; } }

        protected Command _ScanCommand;
        public ICommand ScanCommand { get { return _ScanCommand; } }

        protected Command _FlashlightCommand;
        public ICommand FlashlightCommand { get { return _FlashlightCommand; } }

        protected Command _FullscreenCommand;
        public ICommand FullscreenCommand { get { return _FullscreenCommand; } }
        #endregion

        #region Methoden
        public Android_ScanditViewModel() { }

        /// <summary>
        /// Konstruktor mit Parametern für den Einzelscan / Mehrfachscan
        /// </summary>
        /// <param name="scannedItemService"> ScannedItemServer zum übermitteln von Barcodes</param>
        public Android_ScanditViewModel(ScannedItemService scannedItemService)
        {
            _scannedItemService = scannedItemService;

            BarcodeCapture.BarcodeScanned += OnBarcodeScanned;
            BarcodeCapture.Feedback.Success = new Feedback(Vibration.DefaultVibration);
            
            _ReturnCommand = new Command(OnReturn);
            _ScanCommand = new Command(OnScan);
            _FullscreenCommand = new Command(OnFullscreen);
            _FlashlightCommand = new Command(OnFlashlight);
        }

        /// <summary>
        /// Schaltet die Kamera aus
        /// </summary>
        /// <returns> Befehl zum ausschalten der Kamera</returns>
        public Task OnSleep()
        {
            return Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off) ?? Task.CompletedTask;
        }

        /// <summary>
        /// Schaltet die Kamera an
        /// </summary>
        /// <returns> Befehl zum anschalten der Kamera</returns>
        private Task ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            return Camera?.SwitchToDesiredStateAsync(FrameSourceState.On) ?? Task.CompletedTask;
        }

        /// <summary>
        /// Prüft den Status der Kamera und schaltet diese wenn nötig an
        /// </summary>
        public async Task OnResumeAsync()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    await ResumeFrameSource();
                }
            }
            else
            {
                await ResumeFrameSource();
            }
        }

        /// <summary>
        /// Behandelt das Ereignis wenn ein Barcode gescannt wurde
        /// </summary>
        /// <param name="sender"> Objekt Sender</param>
        /// <param name="args"> Der Barcode der erfasst wurde</param>
        private void OnBarcodeScanned(object? sender, BarcodeCaptureEventArgs args)
        {
            Barcode barcode = args.Session.NewlyRecognizedBarcodes[0];
            SymbologyDescription description = new SymbologyDescription(barcode.Symbology);

            //Erstellt ein Object vom gescannten Barcode
            ScannedItem scannedItem = new ScannedItem()
            {
                Barcodetyp = description.ReadableName,
                ID = ScannedItem.GetNewID(_scannedItemService.ScannedItems),
                Productcode = barcode.Data
            };

            //Geht die Liste durch und prüft ob der Barcode bereits gescannt wurde
            if (ScannedItem.ParseBarcodeList(_scannedItemService.ScannedItems, scannedItem) == true)
            {
                return;
            }
            else
            {
                _scannedItemService.AddScannedItem(scannedItem);
            }

            //Aktiviert bzw deaktiviert den Scan Modus            
            if (!SwitchMode)
            {
                BarcodeCapture.Enabled = false;
            }
            else BarcodeCapture.Enabled = true;

            //Zeigt die Informationen an die der Barcode enthält
            BarcodeSource = "Scanned: " + scannedItem.Productcode + " (" + scannedItem.Barcodetyp + ")";

            //Gibt ein Haptisches Feedback wieder
            Feedback.Emit();
        }

        /// <summary>
        /// Regelt den Zoom Faktor der Kamera
        /// </summary>
        private void Zoom()
        {
            ScanditCamera CurrentCamera = ScanditCamera.GetCamera(CameraPosition.WorldFacing);

            CameraSettings cameraSettings = BarcodeCapture.RecommendedCameraSettings;
            cameraSettings.ZoomFactor = SliderValue;
            cameraSettings.FocusGestureStrategy = FocusGestureStrategy.ManualUntilCapture;

            CurrentCamera.ApplySettingsAsync(cameraSettings);

            Camera = CurrentCamera;
        }

        /// <summary>
        /// Kehrt zurück auf die MainPage
        /// </summary>
        private void OnReturn()
        {
            OnSleep();

            Shell.Current.GoToAsync("//MainPage");
        }

        /// <summary>
        /// Ermöglicht einen erneuten Scan
        /// </summary>
        private void OnScan()
        {
            BarcodeCapture.Enabled = true;
        }

        /// <summary>
        /// Aktiviert die Taschenlampe vom Gerät
        /// </summary>
        private void OnFlashlight()
        {
            if (Camera.TorchAvailable)
            {
                if (Camera.DesiredTorchState == TorchState.Off)     //Taschenlampe an
                {
                    Camera.DesiredTorchState = TorchState.On;       
                }
                else if (Camera.DesiredTorchState == TorchState.On) //Taschenlampe aus
                {
                    Camera.DesiredTorchState = TorchState.Off;      
                }
            }
        }

        /// <summary>
        /// Öffnet den Vollbildmodus
        /// </summary>
        private void OnFullscreen()
        {
            Shell.Current.GoToAsync("//FullscreenScan");
        }
        #endregion
    }
}
