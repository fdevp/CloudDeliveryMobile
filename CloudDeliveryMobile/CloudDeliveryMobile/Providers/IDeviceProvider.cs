using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Providers
{
    public interface IDeviceProvider
    {
        /// <summary>
        /// returns Device ID
        /// </summary>
        /// <returns></returns>
        string DeviceID();

        /// <summary>
        /// returns Device Name
        /// </summary>
        /// <returns></returns>
        string DeviceName();

        /// <summary>
        /// returns internal data directory path
        /// </summary>
        /// <returns></returns>
        string DataPath();

        /// <summary>
        /// returns internal data directory object
        /// </summary>
        /// <returns></returns>
        IFolder DataFolder();

        /// <summary>
        /// checks if file exists
        /// </summary>
        /// <returns></returns>
        bool FileExists(string filename);
    }
}
