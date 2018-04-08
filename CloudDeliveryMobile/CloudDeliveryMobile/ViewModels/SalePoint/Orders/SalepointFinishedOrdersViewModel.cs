using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.SalePoint.Orders;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
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
            try
            {
                await this.salepointOrdersService.GetFinishedOrders();
            }
            catch (HttpUnprocessableEntityException e)
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
            finally
            {
                this.InProgress = false;
            }

        }

        private IMvxNavigationService navigationService;
        private ISalepointOrdersService salepointOrdersService;
    }
}
