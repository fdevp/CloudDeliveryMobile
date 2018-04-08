using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters
{
    public class MonthShortTextValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? date = value as DateTime?;
            string shortText = string.Empty;

            if (!date.HasValue)
                return shortText;


            switch (date.Value.Month)
            {
                case 1:
                    shortText = "STY";
                    break;
                case 2:
                    shortText = "LUT";
                    break;
                case 3:
                    shortText = "MAR";
                    break;
                case 4:
                    shortText = "KWI";
                    break;
                case 5:
                    shortText = "MAJ";
                    break;
                case 6:
                    shortText = "CZE";
                    break;
                case 7:
                    shortText = "LIP";
                    break;
                case 8:
                    shortText = "SIE";
                    break;
                case 9:
                    shortText = "WRZ";
                    break;
                case 10:
                    shortText = "PAŹ";
                    break;
                case 11:
                    shortText = "LIS";
                    break;
                case 12:
                    shortText = "GRU";
                    break;
            }

            return shortText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
