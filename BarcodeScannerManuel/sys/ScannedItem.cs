using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScannerManuel.sys
{
    public class ScannedItem
    {
        public int ID { get; set; } = 0;

        public string Productcode { get; set; } = string.Empty;

        public string Barcodetyp { get; set; } = string.Empty;

        /// <summary>
        /// Weißt einem Barcode eine neue ID zu
        /// </summary>
        /// <param name="scannedItems"> Liste der bereits gescannten Barcodes</param>
        /// <returns> unbenutze ID für die Liste</returns>
        public static int GetNewID(ObservableCollection<ScannedItem> scannedItems)
        {            
            int newID = 0;
            foreach(var item in scannedItems)
            {
                if (item.ID >= newID)
                {
                    newID = item.ID + 1;                    
                }
            }
            return newID;
        }

        /// <summary>
        /// Itteriert durch die ScannedItem List um zu prüfen ob das gescannte Barcode bereits vorhanden ist
        /// </summary>
        /// <param name="scannedItems"> Liste der gescannten Barcodes</param>
        /// <param name="scannedItem"> Aktuell gescannter Barcode</param>
        /// <returns>true wenn es den Barcode bereits gibt; false wenn nicht</returns>
        public static bool ParseBarcodeList(ObservableCollection<ScannedItem> scannedItems, ScannedItem scannedItem)
        {
            foreach (var item in scannedItems)
            {
                if (scannedItem.Productcode == item.Productcode && scannedItem.Barcodetyp == item.Barcodetyp)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
