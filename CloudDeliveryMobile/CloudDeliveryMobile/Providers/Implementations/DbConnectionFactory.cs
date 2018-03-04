using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public DbConnectionFactory(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(dbPath);
        }


        private string dbPath;
    }
}
