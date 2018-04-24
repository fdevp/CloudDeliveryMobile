using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier.Routes
{
    public class CarrierRouteDetailsViewModel : BaseViewModel<int>
    {
        public RouteDetails Route { get; set; }


        public IMvxAsyncCommand CloseFragment
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Close(this);
                });
            }
        }

        public IMvxAsyncCommand ReloadData
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.ErrorOccured = false;
                    await this.InitializeRoute();
                });
            }
        }
        public CarrierRouteDetailsViewModel(IMvxNavigationService navigationService, IRoutesService routesService)
        {
            this.navigationService = navigationService;
            this.routesService = routesService;
        }


        public async override void Start()
        {
            base.Start();
            await this.InitializeRoute();
        }


        public async Task InitializeRoute()
        {
            this.InProgress = true;
            try
            {
                this.Route = await this.routesService.Details(this.orderId);
                RaisePropertyChanged(() => this.Route);
            }
            catch (Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = e.Message;
            }
            this.InProgress = false;
        }


        public override void Prepare(int orderId)
        {
            this.orderId = orderId;
        }

        int orderId;
        IMvxNavigationService navigationService;
        IRoutesService routesService;
    }
}
