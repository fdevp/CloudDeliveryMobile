using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RoutePointActiveModel
    {
        public RoutePoint Point { get; set; }

        public bool Active { get; set; }

        public CarrierSideActiveRouteViewModel ViewModelWrapper { get; set; }

        public RoutePointActiveModel(CarrierSideActiveRouteViewModel vmWrapper)
        {
            this.ViewModelWrapper = vmWrapper;
        }
    }
}
