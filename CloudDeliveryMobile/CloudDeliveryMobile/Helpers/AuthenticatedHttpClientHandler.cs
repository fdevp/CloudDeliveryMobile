using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Helpers
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        public event EventHandler SessionExpired;

        private string refreshToken;
        private string accessToken;

        public AuthenticatedHttpClientHandler(string refreshToken, string accessToken)
        {
            this.refreshToken = refreshToken;
            this.accessToken = accessToken;
        }

        public void SetTokens(string refreshToken, string accessToken)
        {
            this.refreshToken = refreshToken;
            this.accessToken = accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var result = await base.SendAsync(request, cancellationToken);

            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return await RefreshAccessAndSendAsync(request, cancellationToken) ?? result;

            return result;
        }

        private async Task<HttpResponseMessage> RefreshAccessAndSendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var content = AuthDataProvider.CreateTokenSignInContent(refreshToken);
                var authData = await AuthDataProvider.FetchAuthData(content);
                accessToken = authData.AccessToken;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await SendAsync(request, cancellationToken);
            }
            catch
            {
                SessionExpired.Invoke(this, null);
                return null;
            }
        }
    }
}
