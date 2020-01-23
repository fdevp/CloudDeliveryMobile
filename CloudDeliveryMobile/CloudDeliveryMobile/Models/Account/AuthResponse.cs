using Newtonsoft.Json;

namespace CloudDeliveryMobile.Models.Account
{
    public class AuthResponse : SessionData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
