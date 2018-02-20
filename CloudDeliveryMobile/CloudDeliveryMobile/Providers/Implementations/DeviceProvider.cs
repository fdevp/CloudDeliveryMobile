using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Plugin.DeviceInfo;
namespace CloudDeliveryMobile.Providers.Implementations
{
    public class DeviceProvider : IDeviceProvider
    {
        public IFolder DataFolder()
        {
            IFileSystem dataDirPath = FileSystem.Current;
            return dataDirPath.LocalStorage;
        }

        public string DataPath()
        {
            IFileSystem dataDirPath = FileSystem.Current;
            return dataDirPath.LocalStorage.Path;
        }

        public string DeviceID()
        {
            //#if DEBUG
            //            return "debugDEVICE";
            //#endif 
            return CrossDeviceInfo.Current.Id;
        }

        public string DeviceName()
        {
            return CrossDeviceInfo.Current.Platform + " " + CrossDeviceInfo.Current.Version;
        }

        public bool FileExists(string filename)
        {
            return this.DataFolder().CheckExistsAsync(filename).Result == ExistenceCheckResult.FileExists;
        }

      
    }
}
