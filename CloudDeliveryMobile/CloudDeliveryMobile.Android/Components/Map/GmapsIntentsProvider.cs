using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes;

using AndroidNet = Android.Net;

namespace CloudDeliveryMobile.Android.Components.Map
{
    public static class GmapsIntentsProvider
    {
        public static string CreateDirectionsPoint(RoutePoint point)
        {
            if (point.Type == RoutePointType.SalePoint)
            {
                return point.Order.SalepointLatLng.ToGoogleString();
            }
            else if (point.Order.EndLatLng != null)
            {
                return point.Order.EndLatLng.ToGoogleString();
            }
            else
            {
                string validAddress = point.Order.DestinationAddress.Replace(" ", "+");
                string validCity = point.Order.DestinationCity.Replace(" ", "+");
                return string.Concat(validAddress, ",", validCity);
            }

        }

        public static Intent CreatePointIntent(Context ctx,RoutePoint point)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ctx.GetString(Resource.String.gmaps_intent_directions));

            //travelmode
            sb.Append('&');
            sb.Append(ctx.GetString(Resource.String.gmaps_intent_travelmode));

            //destination
            sb.Append("&destination=");
            sb.Append(CreateDirectionsPoint(point));

            AndroidNet.Uri uri = AndroidNet.Uri.Parse(sb.ToString());
            Intent intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NewTask);
            return intent;
        }
    }
}