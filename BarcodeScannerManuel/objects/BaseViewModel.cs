using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScannerManuel.objects
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //protected Page _Page;

        public BaseViewModel(/*Page page*/)
        {
            /*_Page = page;*/
        }

        /// <summary>
        /// ViewModel meldet an die View, dass sich eine Property geändert hat
        /// </summary>
        /// <param name="propertyName">Propertyname angeben ODER beim weglassen wird der aktuelle Propertyname automatisch übernommen</param>
        public void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// ViewModel meldet an die View, dass sich mehrere Propertys geändert haben
        /// </summary>
        /// <param name="propertyNames">Property Namen</param>
        public void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (string p in propertyNames)
                NotifyPropertyChanged(p);
        }

        /// <summary>
        /// ViewModel meldet an die View, dass sich mehrere Propertys mit gleichem Namen aber unterschiedlicher Nummern geändert haben
        /// (z.B. name = "Name", start = 1, ende = 3 für "Name1", "Name2", "Name3")
        /// </summary>
        /// <param name="propertyName">PropertyName (vor der Nummer, z.B. "Name" für "Name1", "Name2" usw)</param>
        /// <param name="numberStart">erste Nummer hinter PropertyName (z.B. "1" für "Name1")</param>
        /// <param name="numberEnd">letzte Nummer hinter PropertyName (z.B. "3" für "Name3")</param>
        public void NotifyPropertyChanged(string propertyName, int numberStart, int numberEnd)
        {
            for (int i = numberStart; i <= numberEnd; i++)
                NotifyPropertyChanged(propertyName + i.ToString());
        }
    }
}
