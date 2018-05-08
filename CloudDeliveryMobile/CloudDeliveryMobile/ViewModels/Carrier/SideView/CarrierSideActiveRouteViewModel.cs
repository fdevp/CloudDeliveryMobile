using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideActiveRouteViewModel : BaseViewModel
    {
        public MvxObservableCollection<RoutePointActiveListViewModel> Points { get; private set; }

        public bool AllPointsPassed
        {
            get
            {
                return this._allPointsPassed;
            }
            set
            {
                this._allPointsPassed = value;
                RaisePropertyChanged(() => this.AllPointsPassed);
            }
        }

        public bool FinishingInProgress
        {
            get
            {
                return this._finishingInProgress;
            }
            set
            {
                this._finishingInProgress = value;
                RaisePropertyChanged(() => this.FinishingInProgress);
            }
        }

        public bool FinishButtonVisible
        {
            get
            {
                return AllPointsPassed && !FinishingInProgress;
            }
        }

        public IMvxAsyncCommand FinishRoute
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.FinishingInProgress = true;
                    RaisePropertyChanged(() => this.FinishButtonVisible);
                    try
                    {
                        await this.routesService.FinishActiveRoute();
                    }
                    catch (ApiException ex)
                    {
                        this.dialogsService.Toast(ex.Message, TimeSpan.FromSeconds(5));
                    }
                    catch (HttpRequestException httpException)
                    {
                        this.dialogsService.Toast("Problem z połączeniem z serwerem.", TimeSpan.FromSeconds(5));
                    }

                    this.FinishingInProgress = false;
                    RaisePropertyChanged(() => this.FinishButtonVisible);
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
                    await this.routesService.ActiveRouteDetails();
                    this.InProgress = false;
                });
            }
        }

        public CarrierSideActiveRouteViewModel(IRoutesService routesService, ICarrierOrdersService ordersService, IMvxNavigationService navigationService, IUserDialogs dialogsService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.dialogsService = dialogsService;

            this.routesService.ActiveRouteUpdated += UpdateActiveRoute;
        }

        public override void Start()
        {
            base.Start();

            if (initialised)
                return;

            this.initialised = true;
            CreatePointsViewModel();
        }


        private void UpdateActiveRoute(object sender, ServiceEvent<CarrierRouteEvents> e)
        {
            switch (e.Type)
            {
                case CarrierRouteEvents.AddedRoute:
                    this.FinishingInProgress = false;
                    this.AllPointsPassed = false;
                    CreatePointsViewModel();
                    break;
                case CarrierRouteEvents.FinishedRoute:
                    this.Points.Clear();
                    break;
            }
        }

        private void CreatePointsViewModel()
        {
            this.Points = new MvxObservableCollection<RoutePointActiveListViewModel>();
            foreach (RoutePoint point in this.routesService.ActiveRoute.Points)
            {
                var pointVM = Mvx.IocConstruct<RoutePointActiveListViewModel>();

                pointVM.OnActiveChange += UpdateActivePoint;

                pointVM.Point = point;
                this.Points.Add(pointVM);
            }

            this.SetActivePoint();

        }

        //method that is called from RoutePointActiveListViewModel after point pass
        private void UpdateActivePoint(int? salepointId)
        {
            this.SetActivePoint();

            //update pickup field
            if (salepointId.HasValue)
            {
                var pickedPoints = this.Points.Where(x => x.Point.Order.SalepointId == salepointId.Value &&
                                   x.Point.Order.PickUpTime.HasValue &&
                                   x.Point.Type == RoutePointType.EndPoint)
                       .ToList();

                foreach (RoutePointActiveListViewModel point in pickedPoints)
                    point.RaisePropertyChanged(() => point.Point);
            }

        }

        private void SetActivePoint()
        {
            //check active point
            var pendingPoints = this.routesService.ActiveRoute.Points.Where(x => !x.PassedTime.HasValue).ToList();
            if (pendingPoints.Count > 0)
            {
                int index = pendingPoints.Min(x => x.Index);

                RoutePointActiveListViewModel point = this.Points.Where(x => x.Point.Index == index).FirstOrDefault();
                point.Active = true;
                point.ShowDetails = true;
                point.RaiseAllPropertiesChanged();
            }
            else
            {
                this.AllPointsPassed = true;
                RaisePropertyChanged(() => this.AllPointsPassed);
                RaisePropertyChanged(() => this.FinishButtonVisible);
            }
        }

        private bool initialised = false;
        private bool _allPointsPassed = false;
        private bool _finishingInProgress = false;

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private ICarrierOrdersService ordersService;
        private IUserDialogs dialogsService;
    }
}
