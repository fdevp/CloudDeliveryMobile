using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class RootCarrierViewModel : BaseViewModel
    {

        public RootCarrierViewModel()
        {
            this.mapVM = Mvx.IocConstruct<CarrierMapViewModel>();
            this.ordersVM = Mvx.IocConstruct<CarrierOrdersViewModel>();
        }

        public CarrierMapViewModel mapVM { get; set; }

        public CarrierOrdersViewModel ordersVM { get; set; }
    }
}
