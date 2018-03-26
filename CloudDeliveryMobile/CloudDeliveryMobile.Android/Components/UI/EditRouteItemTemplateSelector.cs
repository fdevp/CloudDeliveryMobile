using System.Collections.Generic;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace CloudDeliveryMobile.Android.Components.UI
{
    public class EditRouteItemTemplateSelector : IMvxTemplateSelector
    {
        public EditRouteItemTemplateSelector()
        {
            this.listItemLayout = new Dictionary<RoutePointType, int>()
            {
                { RoutePointType.SalePoint, Resource.Layout.route_edit_salepoint_listitem},
                { RoutePointType.EndPoint, Resource.Layout.route_edit_endpoint_listitem }
            };
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int GetItemViewType(object forItemObject)
        {
            var point = (RoutePointEditListItem)forItemObject;
            return this.listItemLayout[point.Type];
        }

        private readonly Dictionary<RoutePointType, int> listItemLayout;
    }
}