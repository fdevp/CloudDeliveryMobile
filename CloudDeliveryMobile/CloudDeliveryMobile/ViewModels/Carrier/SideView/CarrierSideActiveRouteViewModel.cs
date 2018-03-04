using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideActiveRouteViewModel : BaseViewModel
    {
        public RouteDetails Route { get; set; }

        public CarrierSideActiveRouteViewModel(IRoutesService routesService, IOrdersService ordersService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }


        public override void Start()
        {
            base.Start();

            this.Route = this.routesService.ActiveRoute;
        }

        
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
    }
}
