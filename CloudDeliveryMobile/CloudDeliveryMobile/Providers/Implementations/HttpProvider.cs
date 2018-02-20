using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class HttpProvider : IHttpProvider
    {
        public HttpProvider()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string resource, Dictionary<string, string> data = null)
        {
            string query = resource;
            //data
            if (data != null)
            {
                var content = new FormUrlEncodedContent(data);
                query = String.Concat(resource, "?", content.ReadAsStringAsync().Result);
            }
            

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, query))
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                //error response handling
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> PostAsync(string resource, object data = null, bool urlencoded = false)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, resource))
            {
                //data
                if (data != null)
                {
                    if (urlencoded)
                        httpRequestMessage.Content = new FormUrlEncodedContent((Dictionary<string, string>)data);
                    else
                        httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                }

                var response = await httpClient.SendAsync(httpRequestMessage);

                //error response handling
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        public void SetAuthorizationHeader(string token)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpClient httpClient;
    }
}
