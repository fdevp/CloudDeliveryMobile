﻿using Acr.UserDialogs;
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

        public IMvxCommand PassPointDialog
        {
            get
            {

                return new MvxCommand(() =>
                {
                    this.dialogsService.Confirm(this.passPointDialogConfig);
                });
            }
        }

        private async void PassPoint(bool dialogResult)
        {
            if (dialogResult)
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
                        await routesService.PassPoint(this.Point);
                    }

                    this.Active = false;
                    this.ShowDetails = false;

                    this.OnActiveChange?.Invoke(this.Point.Type == RoutePointType.SalePoint ? this.Point.Order.SalepointId : (int?)null);

                }
                catch (Exception e)
                {
                    this.dialogsService.Toast(e.Message, TimeSpan.FromSeconds(5));
                }

                this.InProgress = false;
                RaiseAllPropertiesChanged();
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

        public IMvxCommand MakeSalepointCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    try
                    {
                        this.phoneCallService.MakePhoneCall("", Point.Order.SalepointPhone);
                    }
                    catch (Exception)
                    {
                        dialogsService.Toast("Wystąpił błąd przy wybieraniu numeru", TimeSpan.FromSeconds(5));
                    }
                });
            }
        }


        public IMvxCommand MakeCustomerCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    try
                    {
                        this.phoneCallService.MakePhoneCall("", Point.Order.CustomerPhone);
                    }
                    catch (Exception)
                    {
                        dialogsService.Toast("Wystąpił błąd przy wybieraniu numeru", TimeSpan.FromSeconds(5));
                    }
                    
                });
            }
        }


        public RoutePointActiveListViewModel(IRoutesService routesService, ICarrierOrdersService ordersService, IMvxNavigationService navigationService, IUserDialogs dialogsService, IMvxPhoneCallTask phoneCallService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.phoneCallService = phoneCallService;
            this.dialogsService = dialogsService;

            this.passPointDialogConfig = new ConfirmConfig();
            this.passPointDialogConfig.OkText = "Tak";
            this.passPointDialogConfig.CancelText = "Nie";
            this.passPointDialogConfig.Title = "Zakończ punkt";
            this.passPointDialogConfig.Message = "Czy na pewno chcesz zakończyć ten punkt?";
            this.passPointDialogConfig.OnAction += x => PassPoint(x);
        }


        private void OrderAcceptation(bool dialogResult)
        {

        }

        private ConfirmConfig passPointDialogConfig;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private ICarrierOrdersService ordersService;
        private IUserDialogs dialogsService;
        private IMvxPhoneCallTask phoneCallService;
    }
}
