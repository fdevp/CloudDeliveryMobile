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
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace CloudDeliveryMobile.Android.Components.UI
{
    public class RoutePointDetailsTemplateSelector : IMvxTemplateSelector
    {

        public RoutePointDetailsTemplateSelector()
        {
            this.listItemLayout = new Dictionary<RoutePointType, int>()
            {
                { RoutePointType.SalePoint, Resource.Layout.carrier_routepoint_salepoint_details},
                { RoutePointType.EndPoint, Resource.Layout.carrier_routepoint_endpoint_details }
            };
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int GetItemViewType(object forItemObject)
        {
            var point = (RoutePoint)forItemObject;
            return this.listItemLayout[point.Type];
        }

        private readonly Dictionary<RoutePointType, int> listItemLayout;
    }
}