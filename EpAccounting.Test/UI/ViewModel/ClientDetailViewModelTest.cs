// ///////////////////////////////////
// File: ClientDetailViewModelTest.cs
// Last Change: 16.03.2017  21:31
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.Model.Properties;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class ClientDetailViewModelTest
    {
        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientDetailViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void RaisesPropertyChangedWhenDetailsChange()
        {
            // Arrange
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client());
            clientDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientDetailViewModel.ClientId = 5;
            clientDetailViewModel.Title = ModelFactory.DefaultClientTitle;
            clientDetailViewModel.FirstName = ModelFactory.DefaultClientFirstName;
            clientDetailViewModel.LastName = ModelFactory.DefaultClientLastName;
            clientDetailViewModel.Street = ModelFactory.DefaultClientStreet;
            clientDetailViewModel.HouseNumber = ModelFactory.DefaultClientHouseNumber;
            clientDetailViewModel.PostalCode = ModelFactory.DefaultClientPostalCode;
            clientDetailViewModel.City = ModelFactory.DefaultClientCity;
            clientDetailViewModel.DateOfBirth = ModelFactory.DefaultClientDateOfBirth;
            clientDetailViewModel.MobileNumber = ModelFactory.DefaultClientMobileNumber;
            clientDetailViewModel.PhoneNumber1 = ModelFactory.DefaultClientPhoneNumber1;
            clientDetailViewModel.PhoneNumber2 = ModelFactory.DefaultClientPhoneNumber2;
            clientDetailViewModel.Telefax = ModelFactory.DefaultClientTelefax;
            clientDetailViewModel.Email = ModelFactory.DefaultClientEmail;

            // Assert
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.ClientId);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Title);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.FirstName);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.LastName);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Street);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.HouseNumber);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PostalCode);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.City);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.DateOfBirth);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.MobileNumber);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber1);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber2);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Telefax);
            clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Email);
        }

        [Test]
        public void ReturnZeroWhenClientHasNoBills()
        {
            // Arrange
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client());

            // Assert
            clientDetailViewModel.NumberOfBills.Should().Be(0);
        }

        [Test]
        public void ReturnNumberOfBills()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            Client client = new Client();

            // Act
            client.AddBill(bill);
            client.AddBill(bill);
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            clientDetailViewModel.NumberOfBills.Should().Be(2);
        }

        [Test]
        public void ReturnZeroSalesWhenClientHasNoBills()
        {
            // Arrange
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client());

            // Assert
            clientDetailViewModel.Sales.Should().Be(0);
        }

        [Test]
        public void ReturnSalesWhenClientHasBills()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            client.AddBill(bill);

            // Act
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            clientDetailViewModel.Sales.Should().BeApproximately(66.11, 0.005);
        }

        [Test]
        public void DetectMissingNamePartWhenFirstNameAndLastNameMissing()
        {
            // Act
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client());

            // Assert
            clientDetailViewModel.HasMissingLastName.Should().BeTrue();
        }

        [Test]
        public void DetectMissingNamePartWhenLastNameMissing()
        {
            // Act
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client() { FirstName = "Andre" });

            // Assert
            clientDetailViewModel.HasMissingLastName.Should().BeTrue();
        }

        [Test]
        public void DetectNotMissingNamePart()
        {
            // Act
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(new Client() { FirstName = "Andre", LastName = "Multerer" });

            // Assert
            clientDetailViewModel.HasMissingLastName.Should().BeFalse();
        }

        [Test]
        public void GetClientDetailViewModelString()
        {
            // Arrange
            const int ExpectedId = 5;
            string ExpectedString = string.Format(Resources.Client_ToString, ExpectedId,
                                                  ModelFactory.DefaultClientFirstName, ModelFactory.DefaultClientLastName,
                                                  ModelFactory.DefaultClientStreet, ModelFactory.DefaultClientHouseNumber,
                                                  ModelFactory.DefaultClientPostalCode, ModelFactory.DefaultClientCity);

            // Act
            Client client = ModelFactory.GetDefaultClient();
            client.ClientId = ExpectedId;
            ClientDetailViewModel clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            clientDetailViewModel.ToString().Should().Be(ExpectedString);
        }
    }
}