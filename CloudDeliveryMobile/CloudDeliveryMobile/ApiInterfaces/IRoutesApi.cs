using CloudDeliveryMobile.Models.Routes;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ApiInterfaces
{
    public interface IRoutesApi
    {
        [Get("/api/routes/activeroutedetails")]
        Task<RouteDetails> ActiveRouteDetails();

        [Post("/api/routes/add")]
        Task<RouteDetails> Add([Body]List<RouteEditModel> model);

        [Get("/api/routes/details/{routeId}")]
        Task<RouteDetails> Details(int routeId);

        [Get("/api/routes/finishedlist")]
        Task<List<RouteListItem>> FinishedRoutesList();

        [Put("/api/routes/finish/{routeId}")]
        Task FinishRoute(int routeId);

        [Put("/api/routes/passpoint/{pointId}")]
        Task PassPoint(int pointId);
    }
}
