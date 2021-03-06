﻿using CloudDeliveryMobile.Models.Storage;
using CloudDeliveryMobile.Providers;
using Moq;
using SQLite;

namespace CloudDeliveryMobile.Tests.Mocks
{
    public static class DbConnectionFactoryMocks
    {
        public static Mock<IDbConnectionFactory> StorageDbFactoryMocks()
        {
            var connectionMock =new Mock<SQLiteConnection>();

            MainTable dataTable = new MainTable();
            


            connectionMock.Setup(x => x.Table<MainTable>().FirstOrDefault()).Returns(dataTable);

            var dbfactorymock = new Mock<IDbConnectionFactory>();
            dbfactorymock.Setup(x => x.GetConnection()).Returns(connectionMock.Object);

            return dbfactorymock;
        }

    }
}
