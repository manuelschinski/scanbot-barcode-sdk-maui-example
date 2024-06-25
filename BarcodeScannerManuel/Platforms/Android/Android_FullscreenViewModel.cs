using Android.Graphics;
using BarcodeScannerManuel.objects;
using BarcodeScannerManuel.sys;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Viewfinder;
using System.Windows.Input;
using CameraPosition = Scandit.DataCapture.Core.Source.CameraPosition;
using FocusGestureStrategy = Scandit.DataCapture.Core.Source.FocusGestureStrategy;
using FrameSourceState = Scandit.DataCapture.Core.Source.FrameSourceState;
using ScanditCamera = Scandit.DataCapture.Core.Source.Camera;
using TorchState = Scandit.DataCapture.Core.Source.TorchState;
using Vibration = Scandit.DataCapture.Core.Common.Feedback.Vibration;

namespace BarcodeScannerManuel.Platforms.Android
{
    internal class Android_FullscreenViewModel : BaseViewModel
    {
        #region Variablen
        private readonly ScannedItemService _scannedItemService;

        private IViewfinder viewfinder;

        public ScanditCamera? Camera { get; private set; } = Android_DataCaptureManager.Instance.CurrentCamera;
        public DataCaptureContext DataCaptureContext { get; } = Android_DataCaptureManager.Instance.DataCaptureContext;
        public BarcodeCapture BarcodeCapture { get; } = Android_DataCaptureManager.Instance.BarcodeCapture;
        public Feedback Feedback { get; } = Feedback.DefaultFeedback;

        public IViewfinder Viewfinder
        {
            get { return this.viewfinder; }
            set
            {
                this.viewfinder = value;
                this.NotifyPropertyChanged(nameof(Viewfinder));
            }
        }

        #endregion

        #region Properties
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

        private double _OpacityValue = 0;
        public double OpacityValue
        {
            get => _OpacityValue;
            set
            {
                _OpacityValue = value;
                NotifyPropertyChanged(nameof(OpacityValue));
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
        #endregion

        #region Methoden
        /// <summary>
        /// Konstruktor mit Parametern für den Vollbild Modus
        /// </summary>
        /// <param name="scannedItemService"> ScannedItemServer zum übermitteln von Barcodes</param>
        public Android_FullscreenViewModel(ScannedItemService scannedItemService)
        {
            BarcodeCapture.BarcodeScanned += OnBarcodeScanned;
            BarcodeCapture.Feedback.Success = new Feedback(Vibration.DefaultVibration);

            _scannedItemService = scannedItemService;

            _ReturnCommand = new Command(OnReturn);
            _ScanCommand = new Command(OnScan);
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
            
            ScannedItem scannedItem = new ScannedItem()
            {
                Barcodetyp = description.ReadableName,
                ID = ScannedItem.GetNewID(_scannedItemService.ScannedItems),
                Productcode = barcode.Data
            };

            if (ScannedItem.ParseBarcodeList(_scannedItemService.ScannedItems,scannedItem) == true)
            {
                return;
            }
            else
            {
                _scannedItemService.AddScannedItem(scannedItem);
            }
            
            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new results until
            // the capture mode is enabled again. Note that disabling the capture mode does not stop the camera, the camera
            // continues to stream frames until it is turned off.            
            if (!SwitchMode)
            {
                BarcodeCapture.Enabled = false;
            }
            else BarcodeCapture.Enabled = true;

            // Get the human readable name of the symbology and assemble the result to be shown.
            BarcodeSource = "Scanned: " + scannedItem.Productcode + " (" + scannedItem.Barcodetyp + ")";
            OpacityValue = 1;

            // We also want to emit a feedback (vibration and, if enabled, sound).
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
            Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);
            Shell.Current.GoToAsync("//MainPage");

            OnSleep();
        }

        /// <summary>
        /// Ermöglicht einen erneuten Scan
        /// </summary>
        private void OnScan()
        {
            BarcodeCapture.Enabled = true;
            OpacityValue = 0;
        }

        /// <summary>
        /// Aktiviert die Taschenlampe vom Gerät
        /// </summary>
        private void OnFlashlight()
        {
            if (Camera.TorchAvailable)
            {
                if (Camera.DesiredTorchState == TorchState.Off)
                {
                    Camera.DesiredTorchState = TorchState.On;
                }
                else if (Camera.DesiredTorchState == TorchState.On)
                {
                    Camera.DesiredTorchState = TorchState.Off;
                }
            }
        }
        #endregion
    }
}
