using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.PhoneCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RoutePointActiveListViewModel : BaseViewModel
    {
        public RoutePoint Point { get; set; }

        public bool Active { get; set; }

        public bool ShowDetails { get; set; } = false;

        public Action<int?> OnActiveChange { get; set; }

        public IMvxCommand ToggleDetails
        {
            get
            {
                return new MvxCommand(() =>
                {
                    ShowDetails = !ShowDetails;
                    RaisePropertyChanged(() => this.ShowDetails);
                });
            }
        }

        public IMvxAsyncCommand PassPoint
        {
            get
            {

                return new MvxAsyncCommand(async () =>
                {
                    this.InProgress = true;
                    try
                    {

                        if (this.Point.Type == RoutePointType.SalePoint)
                        {
                            await routesService.PassPoint(this.Point);
                        }
                        else
                        {
                            Task passPoint = routesService.PassPoint(this.Point);
                            Task deliverOrder = ordersService.Delivered(this.Point.Order);

                            await Task.WhenAll(passPoint, deliverOrder);
                        }

                        this.Active = false;
                        this.ShowDetails = false;

                        if (this.OnActiveChange != null)
                            this.OnActiveChange.Invoke(this.Point.Type == RoutePointType.SalePoint ? this.Point.Order.SalepointId : (int?)null);

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

        public IMvxAsyncCommand ShowPickupModal
        {
            get
            {

                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Navigate<int>(typeof(CarrierPickupOrdersViewModel), this.Point.Order.SalepointId);
                });
            }
        }

        public IMvxCommand MakeCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    if (Point.Type == RoutePointType.EndPoint)
                        this.phoneCallService.MakePhoneCall("", Point.Order.CustomerPhone);
                    else if (Point.Type == RoutePointType.SalePoint)
                        this.phoneCallService.MakePhoneCall("", Point.Order.SalepointPhone);
                });
            }
        }

        public RoutePointActiveListViewModel(IRoutesService routesService, IOrdersService ordersService, IMvxNavigationService navigationService, IUserDialogs dialogsService, IMvxPhoneCallTask phoneCallService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.phoneCallService = phoneCallService;
            this.dialogsService = dialogsService;
        }

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
        private IUserDialogs dialogsService;
        private IMvxPhoneCallTask phoneCallService;
    }
}
