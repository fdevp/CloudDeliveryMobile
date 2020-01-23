using CloudDeliveryMobile.Models.Account;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface ISessionProvider
    {
        event EventHandler SessionExpired;

        HttpClient HttpClient { get; }

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
        /// get user data with auth token
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Task<bool> CredentialsSignIn(LoginModel form);

        Task<bool> GoogleSignIn(string authorizationCode, string device);

        Task<bool> RefreshTokenSignIn();

        /// <summary>
        /// removes data about user
        /// </summary>
        /// <returns></returns>
        Task Logout();
    }
}
