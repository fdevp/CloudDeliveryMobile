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

namespace CloudDeliveryMobile.Models.Routes.Edit
{
    public class RoutePointEditModel
    {
        public RoutePointType Type { get; set; }

        public int OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

        public RoutePointEditModel(CarrierSideRouteEditViewModel viewModel)
        {
            this.ViewModelWrapper = viewModel;
        }

        [JsonIgnore]
        public CarrierSideRouteEditViewModel ViewModelWrapper { get; set; }

        [JsonIgnore]
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

        [JsonIgnore]
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
