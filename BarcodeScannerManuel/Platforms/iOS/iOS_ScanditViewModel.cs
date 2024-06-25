using BarcodeScannerManuel.objects;
using System.Windows.Input;

using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Viewfinder;
using Vibration = Scandit.DataCapture.Core.Common.Feedback.Vibration;
using ScanditCamera = Scandit.DataCapture.Core.Source.Camera;

namespace BarcodeScannerManuel.Platforms.iOS
{
    internal class ScanditViewModel : BaseViewModel
    {

        #region Variablen
        internal bool IsFlashlightActiv = false;
        internal int Mode = 1;
        internal IViewfinder ViewfinderStyle = new RectangularViewfinder(RectangularViewfinderStyle.Square, RectangularViewfinderLineStyle.Light);
        
        private IViewfinder viewfinder;

        public ScanditCamera? Camera { get; private set; } = iOS_DataCaptureManager.Instance.CurrentCamera;
        public DataCaptureContext DataCaptureContext { get; } = iOS_DataCaptureManager.Instance.DataCaptureContext;
        public BarcodeCapture BarcodeCapture { get; } = iOS_DataCaptureManager.Instance.BarcodeCapture;
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

        protected Command _ChangeModeCommand;
        public ICommand ChangeModeCommand { get { return _ChangeModeCommand; } }

        protected Command _FullscreenCommand;
        public ICommand FullscrennCommand { get { return _FullscreenCommand; } }
        #endregion

        #region Methoden
        public ScanditViewModel()
        {
            BarcodeScan();            

            _ReturnCommand = new Command(OnReturn);
            _ScanCommand = new Command(OnScan);
            _ChangeModeCommand = new Command(OnChangeMode);
            _FullscreenCommand = new Command(OnFullscreen);
            _FlashlightCommand = new Command(OnFlashlight);
        }

        private void BarcodeScan()
        {
            BarcodeCapture.BarcodeScanned += OnBarcodeScanned;                 
            BarcodeCapture.Feedback.Success = new Feedback(Vibration.DefaultVibration);

            // Rectangular viewfinder with an embedded Scandit logo.
            // The rectangular viewfinder is displayed when the recognition is active and hidden when it is not.
            this.viewfinder = ViewfinderStyle;
        }

        public Task OnSleep()
        {
            return Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off) ?? Task.CompletedTask;
        }

        private Task ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            return Camera?.SwitchToDesiredStateAsync(FrameSourceState.On) ?? Task.CompletedTask;
        }

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

        private void OnBarcodeScanned(object? sender, BarcodeCaptureEventArgs args)
        {
            if (!args.Session.NewlyRecognizedBarcodes.Any())
            {
                return;
            }

            Barcode barcode = args.Session.NewlyRecognizedBarcodes[0];

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new results until
            // the capture mode is enabled again. Note that disabling the capture mode does not stop the camera, the camera
            // continues to stream frames until it is turned off.            
            BarcodeCapture.Enabled = false;

            // If you are not disabling barcode capture here and want to continue scanning, consider
            // setting the codeDuplicateFilter when creating the barcode capture settings to around 500
            // or even -1 if you do not want codes to be scanned more than once.

            // Get the human readable name of the symbology and assemble the result to be shown.
            SymbologyDescription description = new SymbologyDescription(barcode.Symbology);
            BarcodeSource = "Scanned: " + barcode.Data + " (" + description.ReadableName + ")";

            // We also want to emit a feedback (vibration and, if enabled, sound).
            Feedback.Emit();            
        }

        private void Zoom()
        {
            ScanditCamera CurrentCamera = ScanditCamera.GetCamera(CameraPosition.WorldFacing);

            CameraSettings cameraSettings = BarcodeCapture.RecommendedCameraSettings;
            cameraSettings.ZoomFactor = SliderValue;
            cameraSettings.FocusGestureStrategy = FocusGestureStrategy.ManualUntilCapture;

            CurrentCamera.ApplySettingsAsync(cameraSettings);

            Camera = CurrentCamera;
        }

        private void OnReturn()
        {
            Shell.Current.GoToAsync("//MainPage");

            //Kamera ausschalten
        }

        private void OnScan()
        {
            BarcodeCapture.Enabled = true;
        }

        private void OnFlashlight()
        {
            if (Camera.TorchAvailable)
            {
                Camera.DesiredTorchState = TorchState.Auto;
            }
        }

        private void OnChangeMode()
        {
            switch(Mode)
            {
                case 1: 
                    {
                        ViewfinderStyle = new LaserlineViewfinder(LaserlineViewfinderStyle.Animated);
                        
                        Mode = 2;
                        break;
                        }
                case 2:
                    {
                        ViewfinderStyle = new RectangularViewfinder(RectangularViewfinderStyle.Rounded, RectangularViewfinderLineStyle.Light);

                        Mode = 3;
                        break;
                    }
                case 3:
                    {
                        ViewfinderStyle = new AimerViewfinder();

                        Mode = 4;
                        break;
                    }
                case 4:
                    {
                        ViewfinderStyle = new CombinedViewfinder();

                        Mode = 1;
                        break;
                    }
            }
            BarcodeScan();
        }

        private void OnFullscreen()
        {

        }
        #endregion
    }
}
