using CloudDeliveryMobile.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Services
{
    public interface IRoutesService
    {
        RouteDetails ActiveRoute { get; }

        event EventHandler ActiveRouteUpdated;

        Task<RouteDetails> Add(RouteEditModel model);

        Task<RouteDetails> ActiveRouteDetails();

        void FinishActiveRoute();

        Task<RouteDetails> Details(int routeId);

        Task PassPoint(RoutePoint point);
    }
}
