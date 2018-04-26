using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.SalePoint.Orders;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint
{
    public class SalepointFinishedOrdersViewModel : BaseViewModel
    {
        public List<OrderFinishedListItem> Orders
        {
            get
            {
                return this.salepointOrdersService.FinishedOrders;
            }
        }

        public IMvxCommand<OrderFinishedListItem> ShowDetails
        {
            get
            {
                return new MvxCommand<OrderFinishedListItem>(order =>
                {
                    this.navigationService.Navigate<SalepointOrderDetailsViewModel, int>(order.Id);
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

        public SalepointFinishedOrdersViewModel(IMvxNavigationService navigationService, ISalepointOrdersService salepointOrdersService)
        {
            this.navigationService = navigationService;
            this.salepointOrdersService = salepointOrdersService;

            this.salepointOrdersService.FinishedOrdersUpdated += (sender, e) => this.RaisePropertyChanged(() => this.Orders);
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
                await this.salepointOrdersService.GetFinishedOrders();
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
        private ISalepointOrdersService salepointOrdersService;
    }
}
