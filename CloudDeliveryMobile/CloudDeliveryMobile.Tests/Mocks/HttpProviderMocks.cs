using CloudDeliveryMobile.Models.Account;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Tests.Resources;
using Moq;
using System;
using System.Collections.Generic;

namespace CloudDeliveryMobile.Tests.Mocks
{
    public static class HttpProviderMocks
    {
        public static Mock<IHttpProvider> SessionHttpProviderMocks()
        {
            var httpProviderMock = new Mock<IHttpProvider>();


            string tokenUri = string.Concat(ApiResources.Host, "/", ApiResources.UserInfo);
            httpProviderMock.Setup(x => x.GetAsync(
                                                    It.Is<string>(p => p == TestsResources.token),
                                                    It.Is<Dictionary<string, string>>(p => p == null))).ReturnsAsync(TestsResources.token_singin_response);


            string signinUri = string.Concat(ApiResources.Host, "/", ApiResources.Login);
            httpProviderMock.Setup(x => x.PostAsync(
                                                    It.Is<string>(p => p == TestsResources.singin_response),
                                                    It.Is<LoginModel>(p => p.GrantType == TestsResources.grant_type && p.Username == TestsResources.login && p.Password == TestsResources.password),
                                                    It.Is<bool>(p => p == false)
                                                    )).ReturnsAsync(@"{ 'Login': 'carrierkierowca', 'Name': 'carrierkierowca', 'HasRegistered': true, 'LoginProvider': null, 'Roles': '[\'carrier\']' }");

            return httpProviderMock;
        }

        public static Mock<IHttpProvider> OrdersHttpProviderMocks()
        {
            throw new NotImplementedException();
        }

        public static Mock<IHttpProvider> RoutesHttpProviderMocks()
        {
            throw new NotImplementedException();
        }
    }
}
