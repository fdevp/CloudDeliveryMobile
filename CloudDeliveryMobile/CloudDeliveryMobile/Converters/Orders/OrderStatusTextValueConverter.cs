using CloudDeliveryMobile.Models.Enums;
using MvvmCross.Platform.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Converters
{
    public class OrderStatusTextValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OrderStatus? status = value as OrderStatus?;
            string text = string.Empty;
            if (!status.HasValue)
                return text;

            switch (status.Value)
            {
                case OrderStatus.Accepted:
                    text = "Zaakceptowano";
                    break;
                case OrderStatus.InDelivery:
                    text = "W trakcie dowozu";
                    break;
                case OrderStatus.Delivered:
                    text = "Dostarczono";
                    break;
                case OrderStatus.Cancelled:
                    text = "Anulowano";
                    break;
                case OrderStatus.Added:
                    text = "Dodano";
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
