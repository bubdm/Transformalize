﻿using NUnit.Framework;
using Transformalize.Readers;

namespace Transformalize.Test {
    [TestFixture]
    public class TestOutput {

        [Test]
        public void TestCreateSql()
        {
            var process = new ProcessReader("Test").GetProcess();

            var actual = process.CreateOutputSql();

            Assert.AreEqual(@"
CREATE TABLE [dbo].[OrderDetail](
[Address] NVARCHAR(100) NOT NULL,
[City] NVARCHAR(50) NOT NULL,
[Color] NVARCHAR(64) NOT NULL,
[Country] NVARCHAR(2) NOT NULL,
[CustomerKey] INT NOT NULL,
[FirstName] NVARCHAR(50) NOT NULL,
[Gender] NVARCHAR(64) NOT NULL,
[LastName] NVARCHAR(50) NOT NULL,
[LoadDate] DATETIME NOT NULL,
[OrderDate] DATETIME NOT NULL,
[OrderDetailKey] INT NOT NULL,
[OrderKey] INT NOT NULL,
[Price] DECIMAL(10,5) NOT NULL,
[ProductKey] INT NOT NULL,
[ProductName] NVARCHAR(100) NOT NULL,
[Quantity] INT NOT NULL,
[RowVersion] ROWVERSION NOT NULL,
[Size] NVARCHAR(64) NOT NULL,
[State] NVARCHAR(2) NOT NULL,
[TimeKey] INT NOT NULL,
CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED (
[OrderDetailKey] ASC) WITH (IGNORE_DUP_KEY = ON));", actual);
        }

    }
}