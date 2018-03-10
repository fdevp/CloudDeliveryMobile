using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Resources;
using Moq;

namespace CloudDeliveryMobile.Tests.Mocks
{
    public static class StorageProviderMocks
    {
        public static Mock<IStorageProvider> SessionStorageProviderMocks()
        {
            var storageProviderMock = new Mock<IStorageProvider>();

            storageProviderMock.Setup(x => x.Select(It.Is<string>(p => p == DataKeys.Token))).Returns(DataKeys.Token);

            return storageProviderMock;
        }
    }
}
