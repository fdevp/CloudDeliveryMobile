using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
                    catch (Exception e)
                    {
                        this.dialogsService.Toast(e.Message, TimeSpan.FromSeconds(5));
                    }

                    this.FinishingInProgress = false;
                    RaisePropertyChanged(() => this.FinishButtonVisible);
                });
            }
        }

        public CarrierSideActiveRouteViewModel(IRoutesService routesService, ICarrierOrdersService ordersService, IMvxNavigationService navigationService, IUserDialogs dialogsService)
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
            this.Points = new MvxObservableCollection<RoutePointActiveListViewModel>();
            foreach (var item in this.routesService.ActiveRoute.Points)
            {
                var pointVM = Mvx.IocConstruct<RoutePointActiveListViewModel>();

                pointVM.OnActiveChange += UpdateActivePoint;

                pointVM.Point = item;
                this.Points.Add(pointVM);
            }

            this.SetActivePoint();

        }

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
