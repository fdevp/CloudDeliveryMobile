using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface INotificationsProvider
    {
        ConnectionState SocketStatus { get; }

        event EventHandler SocketStatusUpdated;

        event EventHandler<int> CarrierOrderCancelledEvent;
        event EventHandler<OrderCarrier> CarrierOrderAddedEvent;
        event EventHandler<int> CarrierOrderAcceptedEvent;

        event EventHandler<int> SalepointOrderPickedUpEvent;
        event EventHandler<int> SalepointOrderDeliveredEvent;
        event EventHandler<OrderSalepoint> SalepointOrderAcceptedEvent;

        void SetEventHandlers(Roles role);

        void CleanEventHandlers();

        void SetAuthHeader(string token);

        Task StarListening();

        void CleanData();

        void StopListening();
    }
}
