// ///////////////////////////////////
// File: ClientTest.cs
// Last Change: 16.03.2017  22:28
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model
{
    using System;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.Model.Properties;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class ClientTest
    {
        [Test]
        public void CanAddBillToClient()
        {
            // Arrange
            Client client = new Client();
            Bill bill = new Bill();

            // Act
            client.AddBill(bill);

            // Assert
            client.Bills.Should().NotBeEmpty();
        }

        [Test]
        public void ThrowExceptionWhenNullBillShouldBeAddedToClient()
        {
            // Arrange
            Client client = new Client();

            // Act
            Action action = () => client.AddBill(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ClientsEqualIfBothClientsHaveSameValues()
        {
            // Arrange
            Client client1 = ModelFactory.GetDefaultClient();
            Client client2 = ModelFactory.GetDefaultClient();

            // Act
            bool isEqual = client1.Equals(client2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void ClientsEqualIfBothClientsHaveSameValuesAndSameBills()
        {
            // Arrange
            Client client1 = ModelFactory.GetDefaultClient();
            Client client2 = ModelFactory.GetDefaultClient();

            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();

            client1.AddBill(bill1);
            client2.AddBill(bill2);

            // Act
            bool isEqual = client1.Equals(client2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void ClientsUnequalIfClientValuesDiffer()
        {
            // Arrange
            Client client1 = ModelFactory.GetDefaultClient();
            Client client2 = ModelFactory.GetDefaultClient();
            client2.FirstName = "Alfred";

            // Act
            bool isEqual = client1.Equals(client2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void ClientsUnequalIfClientBillsHaveDifferentQuantity()
        {
            // Arrange
            Client client1 = ModelFactory.GetDefaultClient();
            Client client2 = ModelFactory.GetDefaultClient();
            Bill bill = ModelFactory.GetDefaultBill();

            client1.AddBill(bill);

            // Act
            bool isEqual = client1.Equals(client2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void ClientsUnequalIfClientBillsDiffer()
        {
            // Arrange
            Client client1 = ModelFactory.GetDefaultClient();
            Client client2 = ModelFactory.GetDefaultClient();

            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();
            bill2.KindOfBill = KindOfBill.Gutschrift;

            client1.AddBill(bill1);
            client2.AddBill(bill2);

            // Act
            bool isEqual = client1.Equals(client2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void GetClonedClient()
        {
            // Arrange
            Client client = ModelFactory.GetDefaultClient();
            Bill bill = ModelFactory.GetDefaultBill();
            client.AddBill(bill);

            // Act
            Client copiedClient = (Client)client.Clone();

            // Assert
            client.Equals(copiedClient).Should().BeTrue();
        }

        [Test]
        public void ClientsUnequalIfOtherClientIsNull()
        {
            // Arrange
            Client client = ModelFactory.GetDefaultClient();

            // Act
            bool isEqual = client.Equals(null);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void GetHashCodeIfInitialized()
        {
            // Arrange
            Client client = ModelFactory.GetDefaultClient();

            // Act
            Func<int> func = () => client.GetHashCode();

            // Assert
            func.Invoke().Should().NotBe(0);
        }

        [Test]
        public void GetClientString()
        {
            // Arrange
            const int expectedId = 10;
            Client client = ModelFactory.GetDefaultClient();
            client.Id = expectedId;

            // Assert
            client.ToString().Should().Be(string.Format(Resources.Client_ToString, expectedId,
                                                        ModelFactory.DefaultClientFirstName, ModelFactory.DefaultClientLastName,
                                                        ModelFactory.DefaultClientStreet, ModelFactory.DefaultClientHouseNumber,
                                                        ModelFactory.DefaultCityToPostalCodePostalCode, ModelFactory.DefaultCityToPostalCodeCity));
        }
    }
}