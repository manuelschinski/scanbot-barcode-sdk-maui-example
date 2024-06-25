using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScannerManuel.sys
{
    public interface IMessageService
    {
        Task Show(string message);
    }
}
