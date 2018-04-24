using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace CloudDeliveryMobile.Converters
{
    public class PriorityStringValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int priority = (int)value;
            if (priority >= 3)
                return "Bardzo wysoki";
            else if (priority == 2)
                return "Wysoki";
            else
                return "Normalny";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
