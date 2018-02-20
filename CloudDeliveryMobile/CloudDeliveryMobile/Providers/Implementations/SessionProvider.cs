﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                string response = await this.httpProvider.GetAsync(ApiResources.UserInfo);
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(response);
            }
            catch (Exception e)
            {
                throw new InvalidTokenException("Token expired.");
            }

            this.SessionData.access_token = token;

            return true;
        }

        public async Task<bool> SignIn(LoginModel form)
        {
            try
            {
                string response = await this.httpProvider.PostAsync(ApiResources.Login, form.ToDict(),true);
                this.SessionData = JsonConvert.DeserializeObject<SessionData>(response);
            }
            catch (Exception e)
            {
                throw new SignInException("Invalid username or password.");
            }

            this.httpProvider.SetAuthorizationHeader(this.SessionData.access_token);
            this.storageProvider.Insert(DataKeys.Token, this.SessionData.access_token);
            return true;
        }

        public void Logout()
        {
            this.SessionData = null;
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
