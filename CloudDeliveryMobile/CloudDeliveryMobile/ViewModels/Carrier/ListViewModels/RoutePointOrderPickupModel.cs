using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class RoutePointOrderPickupModel : BaseViewModel
    {
        public OrderRouteDetails Order { get; set; }

        public bool PickedUp { get; set; } = false;

        public bool ShowPickUpButton
        {
            get
            {
                return !this.InProgress && !this.PickedUp;
            }
        }

        public IMvxAsyncCommand PickUp
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.InProgress = true;
                    RaisePropertyChanged(() => this.ShowPickUpButton);

                    try
                    {
                        await this.ordersService.Pickup(this.Order);
                        this.PickedUp = true;
                    }
                    catch (Exception e)
                    {
                        this.dialogsService.Toast(e.Message, TimeSpan.FromSeconds(5));
                    }

                    this.InProgress = false;
                    RaiseAllPropertiesChanged();
                });
            }
        }

        public RoutePointOrderPickupModel(IMvxNavigationService navigationService, IRoutesService routesService, IOrdersService ordersService, IUserDialogs dialogsService)
        {
            this.navigationService = navigationService;
            this.routesService = routesService;
            this.ordersService = ordersService;
            this.dialogsService = dialogsService;
        }

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
        private IUserDialogs dialogsService;
    }
}
