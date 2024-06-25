using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScannerManuel.sys
{
    public class MessageService : IMessageService
    {
        private Application? app => Application.Current;

        public Task Show(string message)
        {
            if (app == null)
            {
                return Task.CompletedTask;
            }

            if (app.Dispatcher.IsDispatchRequired)
            {
                return app.Dispatcher.DispatchAsync(() => ShowAction(message));
            }
            else
            {
                return ShowAction(message);
            }
        }

        private Task ShowAction(string message)
        {
            return app?.MainPage?.DisplayAlert("Scandit", message, "OK") ?? Task.CompletedTask;
        }
    }
}
