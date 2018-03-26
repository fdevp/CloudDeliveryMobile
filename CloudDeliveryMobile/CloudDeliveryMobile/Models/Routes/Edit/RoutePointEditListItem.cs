using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class RoutePointEditListItem
    {
        public RoutePointType Type { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public RoutePointEditListItem(CarrierSideRouteEditViewModel viewModel)
        {
            this.ViewModelWrapper = viewModel;
        }

        public CarrierSideRouteEditViewModel ViewModelWrapper { get; set; }

        public IMvxCommand RemoveSalePointRoutePoint
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.ViewModelWrapper.RemoveSalePointRoutePoint.Execute(this);
                });
            }
        }

        public IMvxCommand AddSalePointRoutePoint
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.ViewModelWrapper.AddSalePointRoutePoint.Execute(this);
                });
            }
        }
    }
}
