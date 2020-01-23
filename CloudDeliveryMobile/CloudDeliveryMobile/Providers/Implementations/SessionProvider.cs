using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudDeliveryMobile.Helpers;
using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json.Linq;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class SessionProvider : ISessionProvider
    {
        public event EventHandler SessionExpired;

        public HttpClient HttpClient { get; private set; }

        public SessionData SessionData { get; private set; }

        public SessionProvider(IStorageProvider storageProvider, INotificationsProvider notificationsProvider)
        {
            this.storageProvider = storageProvider;
            this.notificationsProvider = notificationsProvider;

            httpClientHandler = new AuthenticatedHttpClientHandler(null, null);
            httpClientHandler.SessionExpired += (sender, args) =>
            {
                ClearSessionData();
                SessionExpired.Invoke(this, args);
            };

            HttpClient = new HttpClient(httpClientHandler);
            HttpClient.BaseAddress = new Uri(ApiResources.Host);
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

        public async Task<bool> RefreshTokenSignIn()
        {
            refreshToken = storageProvider.SelectOrNull(DataKeys.RefreshToken);
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var content = AuthDataProvider.CreateTokenSignInContent(refreshToken);
            return await SignIn(content);
        }

        public async Task Logout()
        {
            var jsonContent = new JObject(new JProperty("token", refreshToken));
            var httpContent = new StringContent(jsonContent.ToString(), Encoding.UTF8, "application/json");
            await HttpClient.PutAsync("/api/account/cancelToken", httpContent).ContinueWith(t =>
            {
                if (t.IsCompleted && t.Exception == null)
                    ClearSessionData();
            });
        }

        public bool HasSalePointRole()
        {
            return SessionData.InRole("salepoint");
        }

        public bool HasCarrierRole()
        {
            return SessionData.InRole("carrier");
        }

        private async Task<bool> SignIn(FormUrlEncodedContent content)
        {
            var authData = await AuthDataProvider.FetchAuthData(content);
            if (!string.IsNullOrEmpty(authData.RefreshToken))
            {
                storageProvider.Insert(DataKeys.RefreshToken, authData.RefreshToken);
                refreshToken = authData.RefreshToken;
            }

            SessionData = authData;
            notificationsProvider.SetAuthHeader(authData.AccessToken);
            httpClientHandler.SetTokens(refreshToken, authData.AccessToken);
            return true;
        }

        private void ClearSessionData()
        {
            SessionData = null;
            HttpClient.DefaultRequestHeaders.Authorization = null;
            storageProvider.ClearData();
        }

        private IStorageProvider storageProvider;
        private readonly INotificationsProvider notificationsProvider;
        private string refreshToken;
        private AuthenticatedHttpClientHandler httpClientHandler;
    }
}
