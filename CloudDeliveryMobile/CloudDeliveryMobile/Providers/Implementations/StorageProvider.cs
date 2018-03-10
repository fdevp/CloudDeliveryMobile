using CloudDeliveryMobile.Models.Storage;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace CloudDeliveryMobile.Providers.Implementations
{
    public class StorageProvider : IStorageProvider
    {
        public StorageProvider(IDeviceProvider deviceProvider, IDbConnectionFactory dbConnectionFactory)
        {
            this.deviceProvider = deviceProvider;
            this.dbConnectionFactory = dbConnectionFactory;
            
            using (var ctx = dbConnectionFactory.GetConnection())
            {
                //create if not exists
                ctx.CreateTable<MainTable>();
            }
        }

        public void ClearData()
        {
            using (var ctx = dbConnectionFactory.GetConnection())
            {
                ctx.DropTable<MainTable>();
                ctx.CreateTable<MainTable>();
            }
        }

        public void Delete(string key)
        {
            using (var ctx = dbConnectionFactory.GetConnection())
            {
                ctx.DropTable<MainTable>();
                ctx.CreateTable<MainTable>();
            }
        }

        public bool Exists(string key)
        {
            using (var ctx = dbConnectionFactory.GetConnection())
            {
                return ctx.Table<MainTable>().Any(x => x.Key == key);
            }
        }

        public void Insert(string key, object value)
        {
            string json;

            if (value.GetType() == typeof(string))
                json = (string)value;
            else
                json = JsonConvert.SerializeObject(value);

            if (json == null)
                throw new InvalidOperationException("Błąd przy serializowaniu obiektu.");

            using (var ctx = dbConnectionFactory.GetConnection())
            {
                MainTable item = ctx.Table<MainTable>().Where(x => x.Key == key).FirstOrDefault();

                //clear current value
                if (item != null)
                    ctx.Delete(item);

                ctx.Insert(new MainTable { Key = key, Value = json, Updated = DateTime.Now });
            }
        }

        public string Select(string key)
        {
            using (var ctx = dbConnectionFactory.GetConnection())
            {
                MainTable item = ctx.Table<MainTable>().Where(x => x.Key == key).FirstOrDefault();
                if (item == null)
                    throw new NullReferenceException("Do klucza nie ma przypisanej wartości");

                return item.Value;
            }
        }

        private IDeviceProvider deviceProvider;
        private IDbConnectionFactory dbConnectionFactory;
    }
}
