using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScannerManuel.sys
{
    public class ScannedItemService
    {
        public ObservableCollection<ScannedItem> ScannedItems { get; } = new ObservableCollection<ScannedItem>();

        public event Action ScannedItemsUpdated;

        /// <summary>
        /// Fügt einen gescannten Barcode der Liste hinzu
        /// </summary>
        /// <param name="item"> Gescannter Barcode</param>
        public void AddScannedItem(ScannedItem item)
        {
            ScannedItems.Add(item);
            ScannedItemsUpdated?.Invoke();
        }
    }
}
