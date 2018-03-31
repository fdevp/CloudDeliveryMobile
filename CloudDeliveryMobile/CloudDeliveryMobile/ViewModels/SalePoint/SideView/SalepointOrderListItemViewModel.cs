using CloudDeliveryMobile.Models.Orders;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.PhoneCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    this.phoneCallService.MakePhoneCall("", Order.CustomerPhone);;
                });
            }
        }

        public IMvxCommand MakeCarrierCall
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.phoneCallService.MakePhoneCall("", Order.CarrierPhone); ;
                });
            }
        }

        public SalepointOrderListItemViewModel(IMvxPhoneCallTask phoneCallService)
        {
            this.phoneCallService = phoneCallService;
        }

        private IMvxPhoneCallTask phoneCallService;
    }
}
