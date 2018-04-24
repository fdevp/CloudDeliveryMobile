using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters.Signalr
{
    public class SignalrConnectionTextValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = string.Empty;
            ConnectionState? connection = value as ConnectionState?;
            
            if (!connection.HasValue)
                return text;

            switch (connection.Value)
            {
                case ConnectionState.Connected:
                    text = "online";
                    break;
                case ConnectionState.Disconnected:
                    text = "offline";
                    break;
                case ConnectionState.Reconnecting:
                case ConnectionState.Connecting:
                    text = "łączenie";
                    break;
            }

            return text;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
