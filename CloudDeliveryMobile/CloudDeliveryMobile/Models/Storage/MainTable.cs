using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Storage
{
    public class MainTable
    {
        [PrimaryKey, AutoIncrement]
        public int DatabaseId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? Updated { get; set; }
    }
}
