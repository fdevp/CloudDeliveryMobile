using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.Routes;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFinishedRoutesViewModel : BaseViewModel
    {
        public List<RouteListItem> Routes
        {
            get
            {
                return this.routesService.FinishedRoutes;
            }
        }

        public IMvxCommand<RouteListItem> ShowDetails
        {
            get
            {
                return new MvxCommand<RouteListItem>(route =>
                {
                    this.navigationService.Navigate<CarrierRouteDetailsViewModel, int>(route.Id);
                });
            }
        }

        public bool RefreshingInProgress
        {
            get
            {
                return this.refreshingInProgress;
            }
            set
            {
                this.refreshingInProgress = value;
                RaisePropertyChanged(() => this.RefreshingInProgress);
            }
        }

        public IMvxAsyncCommand RefreshList
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.RefreshingInProgress = true;
                    await this.InitializeData();
                    this.RefreshingInProgress = false;
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
                    this.InProgress = true;
                    await this.InitializeData();
                    this.InProgress = false;
                });
            }
        }

        public CarrierFinishedRoutesViewModel(IMvxNavigationService navigationService, IRoutesService routesService)
        {
            this.navigationService = navigationService;
            this.routesService = routesService;

            this.routesService.FinishedRoutesUpdated += (sender, e) =>
            this.RaisePropertyChanged(() => this.Routes);
        }

        public async override void Start()
        {
            base.Start();

            this.InProgress = true;
            await this.InitializeData();
            this.InProgress = false;


        }

        private async Task InitializeData()
        {
            try
            {
                await this.routesService.GetFinishedRoutes();
            }
            catch (ApiException e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = e.Message;
            }
            catch (HttpRequestException httpException)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Problem z połączniem z serwerem";
            }
            catch (Exception unknownException)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Wystąpił nieznany błąd.";
            }
        }

        private bool refreshingInProgress = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
