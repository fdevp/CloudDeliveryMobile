using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using Microsoft.AspNet.SignalR.Client;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class NotificationsProvider : INotificationsProvider
    {
        public ConnectionState SocketStatus { get; private set; } = ConnectionState.Disconnected;
        public event EventHandler SocketStatusUpdated;


        public event EventHandler<int> CarrierOrderCancelledEvent;
        IDisposable CarrierOrderCancelledHandler;

        public event EventHandler<OrderCarrier> CarrierOrderAddedEvent;
        IDisposable CarrierOrderAddedHandler;

        public event EventHandler<int> CarrierOrderAcceptedEvent;
        IDisposable CarrierOrderAcceptedHandler;



        public event EventHandler<int> SalepointOrderPickedUpEvent;
        IDisposable SalepointOrderPickedUpHandler;

        public event EventHandler<int> SalepointOrderDeliveredEvent;
        IDisposable SalepointOrderDeliveredHandler;

        public event EventHandler<OrderSalepoint> SalepointOrderAcceptedEvent;
        IDisposable SalepointOrderAcceptedHandler;


        public NotificationsProvider()
        {
            connection = new HubConnection(ApiResources.Host);
            notificationProxy = connection.CreateHubProxy("NotificationsHub");

            connection.Received += Connection_Received;

            connection.StateChanged += state =>
            {
                this.SocketStatus = connection.State;
                SocketStatusUpdated?.Invoke(this, null);
            };

        }

        private void Connection_Received(string obj)
        {
            int asd = 5;
        }

        public void ClearEventHandlers()
        {
            this.CarrierOrderAcceptedHandler?.Dispose();
            this.CarrierOrderCancelledHandler?.Dispose();
            this.CarrierOrderAddedHandler?.Dispose();

            this.SalepointOrderAcceptedHandler?.Dispose();
            this.SalepointOrderDeliveredHandler?.Dispose();
            this.SalepointOrderPickedUpHandler?.Dispose();
        }

        public void SetAuthHeader(string token)
        {
            this.connection.Headers["Authorization"] = string.Concat("Bearer ", token);
        }

        public void ClearData()
        {
            this.ClearEventHandlers();
            this.StopListening();
            this.connection.Headers.Remove("Authorization");
        }

        public void SetEventHandlers(Roles role)
        {
            this.ClearEventHandlers();

            switch (role)
            {
                case Roles.carrier:
                    CarrierOrderAcceptedHandler = this.notificationProxy.On<int>("OrderAccepted", orderId => CarrierOrderAcceptedEvent?.Invoke(this, orderId));
                    CarrierOrderCancelledHandler = this.notificationProxy.On<int>("OrderCancelled", orderId => CarrierOrderCancelledEvent?.Invoke(this, orderId));
                    CarrierOrderAddedHandler = this.notificationProxy.On<OrderCarrier>("OrderAdded", order => CarrierOrderAddedEvent?.Invoke(this, order));
                    break;
                case Roles.salepoint:
                    SalepointOrderAcceptedHandler = this.notificationProxy.On<OrderSalepoint>("OrderAccepted", order => SalepointOrderAcceptedEvent?.Invoke(this, order));
                    SalepointOrderDeliveredHandler = this.notificationProxy.On<int>("OrderDelivered", orderId => SalepointOrderDeliveredEvent?.Invoke(this, orderId));
                    SalepointOrderPickedUpHandler = this.notificationProxy.On<int>("OrderPickedUp", orderId => SalepointOrderPickedUpEvent?.Invoke(this, orderId));
                    break;

            }
        }

        public async Task StarListening()
        {
            await connection.Start().ContinueWith(t =>
            {

            });
        }

        public void StopListening()
        {
            connection.Stop();
        }

        private readonly HubConnection connection;

        private IHubProxy notificationProxy;
    }
}
