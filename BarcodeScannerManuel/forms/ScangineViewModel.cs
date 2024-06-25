using BarcodeScannerManuel.objects;
using BarcodeScannerManuel.sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BarcodeScannerManuel.forms
{
    internal class ScangineViewModel : BaseViewModel
    {
        #region Variablen
        internal bool IsFlashlightActiv = false;

        #endregion
        #region Properties
        private int _SliderValue = 0;
        public int SliderValue
        {
            get => _SliderValue;
            set
            {
                if (value != _SliderValue)
                {
                    _SliderValue = value;
                    _SliderString = value.ToString();
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
        #endregion
        #region Action
        protected Command _ReturnCommand;
        public ICommand ReturnCommand { get { return _ReturnCommand; } }

        protected Command _ScanCommand;
        public ICommand ScanCommand { get { return _ScanCommand; } }

        protected Command _FlashlightCommand;
        public ICommand FlashlightCommand {  get { return _FlashlightCommand; } }
        #endregion

        #region Methoden
        public ScangineViewModel() 
        {
            _ReturnCommand = new Command(OnReturn);
            _ScanCommand = new Command(OnScan);
            _FlashlightCommand = new Command(OnFlashlight);
        }

        private void OnReturn()
        {
            Shell.Current.GoToAsync("//MainPage");
        }

        private void OnScan()
        {

        }
        
        private void OnFlashlight()
        {
            IsFlashlightActiv = !IsFlashlightActiv;
        }
        #endregion
    }
}
