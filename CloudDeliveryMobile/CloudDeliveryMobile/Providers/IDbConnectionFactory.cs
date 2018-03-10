using SQLite;

namespace CloudDeliveryMobile.Providers
{
    public interface IDbConnectionFactory
    {
        SQLiteConnection GetConnection();
    }
}
