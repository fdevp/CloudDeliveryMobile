using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.Navigation;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierSideViewViewModel : BaseViewModel
    {
        public CarrierSideRouteEditViewModel routeEditVM { get; set; }

        public CarrierSideTakeOrdersViewModel takeOrdersVM { get; set; }

        public CarrierSideActiveRouteViewModel activeRouteVM { get; set; }


        public CarrierSideViewViewModel(IRoutesService routesService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;

            this.routeEditVM = Mvx.IocConstruct<CarrierSideRouteEditViewModel>();
            this.takeOrdersVM = Mvx.IocConstruct<CarrierSideTakeOrdersViewModel>();
            this.activeRouteVM = Mvx.IocConstruct<CarrierSideActiveRouteViewModel>();

            this.routesService.ActiveRouteUpdated += onActiveRouteUpdated;
        }

        private async void onActiveRouteUpdated(object sender, EventArgs e)
        {

            if (this.routesService.ActiveRoute == null && (activeChildViewModel == null || activeChildViewModel.GetType() != typeof(CarrierSideRouteEditViewModel)))
            {
                await this.navigationService.Close(this.activeChildViewModel);
                await this.navigationService.Navigate(this.routeEditVM);
            }
            else if (this.routesService.ActiveRoute != null && (activeChildViewModel == null || activeChildViewModel.GetType() != typeof(CarrierSideActiveRouteViewModel)))
            {
                await this.navigationService.Close(this.activeChildViewModel);
                await this.navigationService.Navigate(this.activeRouteVM);
            }
        }

        public async override void Start()
        {
            base.Start();

            if (this.initialised)
                return;

            this.initialised = true;
            await this.InitializeActiveRoute();
        }

        public async Task InitializeActiveRoute()
        {
            this.InProgress = true;

            try
            {
                await this.routesService.ActiveRouteDetails();
            }
            catch (HttpUnprocessableEntityException e) // no active routes
            {
                this.activeChildViewModel = this.routeEditVM;
                await this.navigationService.Navigate(this.routeEditVM);
            }
            catch (HttpRequestException e) //no connection
            {
                this.Error.Occured = true;
                this.Error.Message = "Problem z połączeniem z serwerem.";
            }
            finally
            {
                this.InProgress = false;
            }
        }

        public bool initialised = false;
        private BaseViewModel activeChildViewModel;

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
