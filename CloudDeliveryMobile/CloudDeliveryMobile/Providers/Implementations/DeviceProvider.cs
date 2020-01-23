using MvvmCross.Core.ViewModels;
using PCLStorage;
using Plugin.DeviceInfo;
namespace CloudDeliveryMobile.Providers.Implementations
{
    public class DeviceProvider : IDeviceProvider
    {
        public MvxViewModel RootViewModel { get; set; }

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
            return $" {CrossDeviceInfo.Current.Manufacturer} {CrossDeviceInfo.Current.Model} {CrossDeviceInfo.Current.Platform} {CrossDeviceInfo.Current.Version}";
        }

        public bool FileExists(string filename)
        {
            return this.DataFolder().CheckExistsAsync(filename).Result == ExistenceCheckResult.FileExists;
        }

      
    }
}
