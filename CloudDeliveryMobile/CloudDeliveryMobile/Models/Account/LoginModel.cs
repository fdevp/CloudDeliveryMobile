using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Account
{
    public class LoginModel
    {
        public string GrantType { get; set; } = "password";

        public string Username { get; set; } = String.Empty;

        public string Password { get; set; } = String.Empty;

        public Dictionary<string,string> ToDict()
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("grant_type", this.GrantType);
            dictionary.Add("username", this.Username);
            dictionary.Add("password", this.Password);

            return dictionary;
        }
    }
}
