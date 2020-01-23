using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Helpers
{
    public static class AuthDataProvider
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static FormUrlEncodedContent CreateTokenSignInContent(string refreshToken)
        {
            var form = new Dictionary<string, string> { { "grant_type", "refresh_token" }, { "refresh_token", refreshToken } };
            return new FormUrlEncodedContent(form);
        }

        public static async Task<AuthResponse> FetchAuthData(FormUrlEncodedContent content)
        {
            string url = string.Concat(ApiResources.Host, "/", ApiResources.Login);
            HttpResponseMessage response = await new HttpClient().PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new SignInException(responseContent);

            return JsonConvert.DeserializeObject<AuthResponse>(responseContent);
        }
    }
}
