using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierPickupOrdersViewModel : BaseViewModel<int>
    {
        public List<RoutePointOrderPickupModel> AcceptedOrders { get; set; }

        public string SalepointName { get; set; }

        public IMvxAsyncCommand CloseFragment
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Close(this);
                });
            }
        }

        public CarrierPickupOrdersViewModel(IMvxNavigationService navigationService, IRoutesService routesService)
        {
            this.navigationService = navigationService;
            this.routesService = routesService;
        }

        public override void Prepare(int salepointId)
        {
            AcceptedOrders = new List<RoutePointOrderPickupModel>();

            var salepointOrders = this.routesService.ActiveRoute.Points.Where(x => x.Order.Status == OrderStatus.Accepted &&
                                                                                   x.Type == RoutePointType.EndPoint &&
                                                                                   x.Order.SalepointId == salepointId).ToList();

            foreach(RoutePoint point in salepointOrders)
            {
                var pointVM = Mvx.IocConstruct<RoutePointOrderPickupModel>();
                pointVM.Order = point.Order;

                AcceptedOrders.Add(pointVM);
            }

            var anySalepointOrder = this.routesService.ActiveRoute.Points.Where(x => x.Order.SalepointId == salepointId).FirstOrDefault();
            this.SalepointName = anySalepointOrder.Order.SalepointName;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
        }

        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
    }
}
