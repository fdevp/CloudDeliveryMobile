using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace CloudDeliveryMobile.Converters
{
    public class TimeStringValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var datetime = value as DateTime?;
            if(datetime.HasValue)
                return datetime.Value.ToString("H:mm");
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
