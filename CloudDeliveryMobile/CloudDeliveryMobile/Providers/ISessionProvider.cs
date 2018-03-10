using CloudDeliveryMobile.Models.Account;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface ISessionProvider
    {

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
        Task<bool> SignIn(LoginModel form);


        /// <summary>
        /// removes data about user
        /// </summary>
        /// <returns></returns>
        void Logout();
    }
}
