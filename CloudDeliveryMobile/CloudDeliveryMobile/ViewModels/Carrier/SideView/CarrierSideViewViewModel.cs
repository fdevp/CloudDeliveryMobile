using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierSideViewViewModel : BaseViewModel
    {
        public CarrierSideRouteEditViewModel editRouteVM { get; set; }

        public CarrierSideActiveRouteViewModel activeRouteVM { get; set; }

        public IMvxAsyncCommand InitializeSideViewContent
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (!dataInitialised)
                    {

                        if (this.routesService.ActiveRoute != null)
                        {
                            await this.navigationService.Navigate(this.activeRouteVM);
                            this.currentChildViewModel = this.activeRouteVM;
                        }
                        else
                        {
                            await this.navigationService.Navigate(this.editRouteVM);
                            this.currentChildViewModel = this.editRouteVM;
                        }

                        dataInitialised = true;

                    }
                });
            }
        }

        public CarrierSideViewViewModel(IRoutesService routesService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;

            this.editRouteVM = Mvx.IocConstruct<CarrierSideRouteEditViewModel>();
            this.activeRouteVM = Mvx.IocConstruct<CarrierSideActiveRouteViewModel>();

            this.routesService.ActiveRouteUpdated += onActiveRouteUpdated;
        }

        private async void onActiveRouteUpdated(object sender, ServiceEvent<CarrierRouteEvents> e)
        {
            //first init should be handled by InitializeSideViewContent method, after view load
            if (!dataInitialised)
                return;

            //handle only major route events
            if (e.Type == CarrierRouteEvents.CancelledPoint || e.Type == CarrierRouteEvents.PassedPoint)
                return;

            this.InProgress = true;

            BaseViewModel vmToLoad = e.Type == CarrierRouteEvents.AddedRoute ? this.activeRouteVM : (BaseViewModel)this.editRouteVM;
            Type currentChildType = this.currentChildViewModel?.GetType();

            if (currentChildType != vmToLoad.GetType())
            {
                await this.navigationService.Navigate(vmToLoad).ContinueWith(t =>
                {
                    this.InProgress = false;
                });
                this.currentChildViewModel = vmToLoad;
            }
        }

        public BaseViewModel currentChildViewModel;
        private bool dataInitialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
