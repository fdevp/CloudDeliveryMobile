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
    public class ActiveRouteItemTemplateSelector : IMvxTemplateSelector
    {

        public ActiveRouteItemTemplateSelector()
        {
            this.listItemLayout = new Dictionary<RoutePointType, int>()
            {
                 { RoutePointType.SalePoint, Resource.Layout.route_active_salepoint_listitem},
                { RoutePointType.EndPoint, Resource.Layout.route_active_endpoint_listitem }
            };
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int GetItemViewType(object forItemObject)
        {
            var point = (RoutePointActiveListViewModel)forItemObject;
            return this.listItemLayout[point.Point.Type];
        }

        private readonly Dictionary<RoutePointType, int> listItemLayout;
    }
}