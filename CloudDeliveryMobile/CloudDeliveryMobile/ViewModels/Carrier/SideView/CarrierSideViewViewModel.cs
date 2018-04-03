using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.Navigation;
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

        public CarrierSideViewViewModel(IRoutesService routesService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;

            this.editRouteVM = Mvx.IocConstruct<CarrierSideRouteEditViewModel>();
            this.activeRouteVM = Mvx.IocConstruct<CarrierSideActiveRouteViewModel>();

            this.routesService.ActiveRouteUpdated += onActiveRouteUpdated;
        }

        private async void onActiveRouteUpdated(object sender, EventArgs e)
        {
            BaseViewModel vmToLoad = this.routesService.ActiveRoute != null ? this.activeRouteVM : (BaseViewModel)this.editRouteVM;
            Type currentChildType = this.currentChildViewModel?.GetType();

            if (currentChildType != vmToLoad.GetType())
            {
                await this.navigationService.Navigate(vmToLoad); 
                this.currentChildViewModel = vmToLoad;
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
                this.currentChildViewModel = this.editRouteVM;
                await this.navigationService.Navigate(this.editRouteVM);
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
        public BaseViewModel currentChildViewModel;

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
