using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;
using Refit;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class SessionProvider : ISessionProvider
    {
        public HttpClient HttpClient { get; private set; }

        public SessionProvider(IStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
            this.HttpClient = new HttpClient();
            this.HttpClient.BaseAddress = new Uri(ApiResources.Host);
        }

        public SessionData SessionData { get; private set; }

        public async Task<bool> CheckToken()
        {
            string token;
            try
            {
                token = this.storageProvider.Select(DataKeys.Token);
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception)
            {
                throw new NullReferenceException("Brak danych do logowania.");
            }

            string url = string.Concat(ApiResources.Host, "/", ApiResources.UserInfo);
            HttpResponseMessage response = await this.HttpClient.GetAsync(url);
            string responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(responseContent);
                this.SessionData.access_token = token;
                return true;
            }
            else
            {
                this.HttpClient.DefaultRequestHeaders.Authorization = null;
                this.storageProvider.Delete(DataKeys.Token);
                throw new InvalidTokenException("Sesja wygasła.");
            }
        }

        public async Task<bool> CredentialsSignIn(LoginModel form)
        {
            var content = new FormUrlEncodedContent(form.ToDict());
            return await SignIn(content);
        }

        public async Task<bool> GoogleSignIn(string authorizationCode, string device)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("grant_type", "authorization_code");
            parameters.Add("code", authorizationCode);
            parameters.Add("client_id", ConstantValues.google_auth_client_id);
            parameters.Add("device", device);

            var content = new FormUrlEncodedContent(parameters);
            return await SignIn(content);
        }

        private async Task<bool> SignIn(FormUrlEncodedContent content)
        {
            string url = string.Concat(ApiResources.Host, "/", ApiResources.Login);
            HttpResponseMessage response = await this.HttpClient.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(responseContent);
            }
            else
            {
                throw new SignInException(responseContent);
            }


            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.SessionData.access_token);
            this.storageProvider.Insert(DataKeys.Token, this.SessionData.access_token);
            return true;
        }

        public void Logout()
        {
            this.SessionData = null;
            this.HttpClient.DefaultRequestHeaders.Authorization = null;
            this.storageProvider.ClearData();
        }

        public bool HasSalePointRole()
        {
            return this.SessionData.InRole("salepoint");
        }

        public bool HasCarrierRole()
        {
            return this.SessionData.InRole("carrier");
        }
        private IStorageProvider storageProvider;
    }
}
