using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideActiveRouteViewModel : BaseViewModel
    {
        public List<RoutePointActiveModel> Points { get; set; }

        public CarrierSideActiveRouteViewModel(IRoutesService routesService, IOrdersService ordersService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }

        public override void Start()
        {
            base.Start();

            if (initialised)
                return;

            this.initialised = true;
            CreatePointsViewModel();
        }

        private void CreatePointsViewModel()
        {
            this.Points = new List<RoutePointActiveModel>();
            foreach (var item in this.routesService.ActiveRoute.Points)
            {
                var pointVM = new RoutePointActiveModel(this);
                pointVM.Point = item;
                this.Points.Add(pointVM);
            }

            //check active point
            int? index = this.routesService.ActiveRoute.Points.Where(x => !x.PassedTime.HasValue).DefaultIfEmpty(null).Min(x => x.Index);
            if (index.HasValue)
            {
                RoutePointActiveModel point = this.Points.Where(x => x.Point.Index == index).FirstOrDefault();
                point.Active = true;
            }

        }

        public bool initialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
    }
}
