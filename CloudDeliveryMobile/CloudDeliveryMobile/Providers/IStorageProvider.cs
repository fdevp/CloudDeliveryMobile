namespace CloudDeliveryMobile.Providers
{
    public interface IStorageProvider
    {
        /// <summary>
        /// select value of key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Select(string key);

        /// <summary>
        /// serialize object and create key with serialized value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Insert(string key, object value);

        /// <summary>
        /// remove key
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        /// <summary>
        /// checks whether key exists in db
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// drop database
        /// </summary>
        void ClearData();
    }
}
