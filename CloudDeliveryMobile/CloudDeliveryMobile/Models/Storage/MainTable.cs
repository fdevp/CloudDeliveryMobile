using System;
using SQLite;

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
