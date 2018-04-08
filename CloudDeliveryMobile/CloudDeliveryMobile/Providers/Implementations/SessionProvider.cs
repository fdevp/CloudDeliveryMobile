using System;
using System.Threading.Tasks;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Resources;
using Newtonsoft.Json;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class SessionProvider : ISessionProvider
    {
        public SessionProvider(IHttpProvider httpProvider, IStorageProvider storageProvider)
        {
            this.httpProvider = httpProvider;
            this.storageProvider = storageProvider;
        }

        public SessionData SessionData { get; private set; }

        public async Task<bool> CheckToken()
        {
            string token;
            try
            {
                token = this.storageProvider.Select(DataKeys.Token);
                this.httpProvider.SetAuthorizationHeader(token);
            }
            catch (Exception)
            {
                throw new NullReferenceException("Brak danych do logowania.");
            }

            try
            {
                string response = await this.httpProvider.GetAsync(httpProvider.AbsoluteUri(ApiResources.UserInfo));
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(response);
            }
            catch (HttpUnprocessableEntityException e)
            {
                throw new InvalidTokenException("Sesja wygasła");
            }

            this.SessionData.access_token = token;

            return true;
        }

        public async Task<bool> SignIn(LoginModel form)
        {
            try
            {
                string response = await this.httpProvider.PostAsync(httpProvider.AbsoluteUri(ApiResources.Login), form.ToDict(),true);
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(response);
            }
            catch (HttpUnprocessableEntityException e)
            {
                throw new SignInException("Podano nieprawidłowe dane");
            }

            this.httpProvider.SetAuthorizationHeader(this.SessionData.access_token);
            this.storageProvider.Insert(DataKeys.Token, this.SessionData.access_token);
            return true;
        }

        public void Logout()
        {
            this.SessionData = null;
            this.httpProvider.SetAuthorizationHeader(string.Empty);
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

        private IHttpProvider httpProvider;
        private IStorageProvider storageProvider;
    }
}
