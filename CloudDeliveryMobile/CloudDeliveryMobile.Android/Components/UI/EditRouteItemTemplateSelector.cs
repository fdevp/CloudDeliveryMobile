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
    public class EditRouteItemTemplateSelector : IMvxTemplateSelector
    {

        public EditRouteItemTemplateSelector()
        {
            this.listItemLayout = new Dictionary<int, int>()
            {
                { (int)RoutePointType.SalePoint, Resource.Layout.carrier_edit_salepoint_listitem },
                { (int)RoutePointType.EndPoint, Resource.Layout.carrier_edit_endpoint_listitem }
            };
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int GetItemViewType(object forItemObject)
        {
            var point = (RoutePoint)forItemObject;
            return this.listItemLayout[(int)point.Type];
        }

        private readonly Dictionary<int, int> listItemLayout;
    }
}