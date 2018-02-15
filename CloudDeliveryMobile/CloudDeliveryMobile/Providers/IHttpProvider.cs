using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface IHttpProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> GetAsync(string resource, Dictionary<string, string> data = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="authHeader">Include authorization header with token i request headers</param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> PostAsync(string resource, object data = null, bool urlencoded = false);


        /// <summary>
        /// add to default headers authorization header based on bearer token
        /// </summary>
        /// <param name="token"></param>
        void SetAuthorizationHeader(string token);
    }
}
