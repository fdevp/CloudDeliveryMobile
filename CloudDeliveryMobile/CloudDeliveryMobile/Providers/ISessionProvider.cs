﻿using CloudDeliveryMobile.Models.Account;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface ISessionProvider
    {
        HttpClient HttpClient { get;}

        SessionData SessionData { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool HasSalePointRole();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool HasCarrierRole();

        /// <summary>
        /// try use token to get data
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckToken();

        /// <summary>
        /// get user data with auth token
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<bool> CredentialsSignIn(LoginModel form);

        Task<bool> GoogleSignIn(string authorizationCode, string device);

        /// <summary>
        /// removes data about user
        /// </summary>
        /// <returns></returns>
        void Logout();
    }
}
