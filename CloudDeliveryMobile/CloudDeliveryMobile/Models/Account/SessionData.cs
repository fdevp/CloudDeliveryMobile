using Newtonsoft.Json;
using System.Linq;

namespace CloudDeliveryMobile.Models.Account
{
    public class SessionData
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public string[] RolesArray { get; set; }

        //serialized string array
        public string Roles
        {
            get
            {
                return this.roles;
            }
            set
            {
                this.roles = value;
                this.RolesArray = JsonConvert.DeserializeObject<string[]>(value);
            }
        }
        

        public bool InRole(string role)
        {
            return this.RolesArray.Any(x => x == role);
        }

        private string roles;
    }
}
