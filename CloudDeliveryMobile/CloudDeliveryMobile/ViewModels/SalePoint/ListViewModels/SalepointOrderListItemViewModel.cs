using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.PhoneCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint
{
    public class SalepointOrderListItemViewModel : BaseViewModel
    {
        public OrderSalepoint Order { get; set; }

        public bool ShowDetails { get; set; } = false;

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

        public IMvxCommand MakeCustomerCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.MakeCall(this.Order.CustomerPhone);
                });
            }
        }

        public IMvxCommand MakeCarrierCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.MakeCall(this.Order.CarrierPhone);
                });
            }
        }

        public IMvxCommand CancelOrder
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.dialogsService.Confirm(this.cancelationDialogConfig);
                });
            }
        }


        private void MakeCall(string phoneNumber)
        {
            try
            {
                var correctNumber = Regex.Replace(phoneNumber, "[^0-9.]", ""); ;
                this.phoneCallService.MakePhoneCall(correctNumber, Order.CustomerPhone); ;
            }
            catch (Exception e)
            {
                this.dialogsService.Toast("Wystąpił problem przy wybieraniu numeru", TimeSpan.FromSeconds(4));
            }
        }

        private async void OrderCancellation(bool dialogResult)
        {
            if (dialogResult)
            {
                this.InProgress = true;

                try
                {
                    await this.salepointOrdersService.Cancel(this.Order);
                    return;
                }
                catch (HttpUnprocessableEntityException e)
                {
                    this.ErrorOccured = true;
                    this.ErrorMessage = e.Message;
                    this.dialogsService.Toast(string.Concat("Błąd, ", this.ErrorMessage), TimeSpan.FromSeconds(4));
                }
                catch (HttpRequestException)
                {
                    this.ErrorOccured = true;
                    this.ErrorMessage = "Problem z połączniem z serwerem";
                }
                catch(Exception unknownException)
                {
                    this.ErrorOccured = true;
                    this.ErrorMessage = string.Concat(unknownException.Message);
                }
                finally
                {
                    this.InProgress = false;
                }


                this.dialogsService.Toast(string.Concat("Błąd, ", this.ErrorMessage), TimeSpan.FromSeconds(4));

            }
        }

        public SalepointOrderListItemViewModel(IMvxPhoneCallTask phoneCallService, ISalepointOrdersService salepointOrdersService, IUserDialogs dialogsService)
        {
            this.phoneCallService = phoneCallService;
            this.salepointOrdersService = salepointOrdersService;
            this.dialogsService = dialogsService;

            this.cancelationDialogConfig = new ConfirmConfig();
            this.cancelationDialogConfig.OkText = "Tak";
            this.cancelationDialogConfig.CancelText = "Nie";
            this.cancelationDialogConfig.Title = "Anulowanie zamówienia";
            this.cancelationDialogConfig.Message = "Czy na pewno chcesz anulować zamówienie?";
            this.cancelationDialogConfig.OnAction += OrderCancellation;
        }

        private ConfirmConfig cancelationDialogConfig;
        private IMvxPhoneCallTask phoneCallService;
        private ISalepointOrdersService salepointOrdersService;
        private IUserDialogs dialogsService;
    }
}
