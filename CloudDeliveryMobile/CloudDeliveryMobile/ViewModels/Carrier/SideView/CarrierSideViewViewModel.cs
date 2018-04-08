using CloudDeliveryMobile.Helpers.Exceptions;
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
                    if (initialised)
                        return;

                    initialised = true;

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


        public override void Start()
        {
            base.Start();
        }

        private async void onActiveRouteUpdated(object sender, EventArgs e)
        {
            if (!initialised)
                return;

            BaseViewModel vmToLoad = this.routesService.ActiveRoute != null ? this.activeRouteVM : (BaseViewModel)this.editRouteVM;
            Type currentChildType = this.currentChildViewModel?.GetType();

            if (currentChildType != vmToLoad.GetType())
            {
                await this.navigationService.Navigate(vmToLoad);
                this.currentChildViewModel = vmToLoad;
            }
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            this.initialised = false;
        }

        public BaseViewModel currentChildViewModel;
        private bool initialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
