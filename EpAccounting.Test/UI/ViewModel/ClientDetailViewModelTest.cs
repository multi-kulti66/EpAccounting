// ///////////////////////////////////
// File: ClientDetailViewModelTest.cs
// Last Change: 23.08.2017  20:22
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.Model.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class ClientDetailViewModelTest
    {
        #region Fields

        private ClientDetailViewModel clientDetailViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.clientDetailViewModel = new ClientDetailViewModel(new Client());
        }

        [TearDown]
        public void Cleanup()
        {
            this.clientDetailViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientDetailViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void RaisesPropertyChangedWhenDetailsChange()
        {
            // Arrange
            this.clientDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientDetailViewModel.ClientId = 5;
            this.clientDetailViewModel.Title = ModelFactory.DefaultClientTitle;
            this.clientDetailViewModel.FirstName = ModelFactory.DefaultClientFirstName;
            this.clientDetailViewModel.LastName = ModelFactory.DefaultClientLastName;
            this.clientDetailViewModel.Street = ModelFactory.DefaultClientStreet;
            this.clientDetailViewModel.HouseNumber = ModelFactory.DefaultClientHouseNumber;
            this.clientDetailViewModel.PostalCode = ModelFactory.DefaultClientPostalCode;
            this.clientDetailViewModel.City = ModelFactory.DefaultClientCity;
            this.clientDetailViewModel.DateOfBirth = ModelFactory.DefaultClientDateOfBirth;
            this.clientDetailViewModel.MobileNumber = ModelFactory.DefaultClientMobileNumber;
            this.clientDetailViewModel.PhoneNumber1 = ModelFactory.DefaultClientPhoneNumber1;
            this.clientDetailViewModel.PhoneNumber2 = ModelFactory.DefaultClientPhoneNumber2;
            this.clientDetailViewModel.Telefax = ModelFactory.DefaultClientTelefax;
            this.clientDetailViewModel.Email = ModelFactory.DefaultClientEmail;

            // Assert
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.ClientId);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Title);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.FirstName);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.LastName);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Street);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.HouseNumber);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PostalCode);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.City);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.DateOfBirth);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.MobileNumber);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber1);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber2);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Telefax);
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Email);
        }

        [Test]
        public void ReturnZeroWhenClientHasNoBills()
        {
            // Assert
            this.clientDetailViewModel.NumberOfBills.Should().Be(0);
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
            this.clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            this.clientDetailViewModel.NumberOfBills.Should().Be(2);
        }

        [Test]
        public void ReturnZeroSalesWhenClientHasNoBills()
        {
            // Assert
            this.clientDetailViewModel.Sales.Should().Be(0);
        }

        [Test]
        public void ReturnSalesWhenClientHasBills()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            client.AddBill(bill);

            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            this.clientDetailViewModel.Sales.Should().BeApproximately(66.11, 0.005);
        }

        [Test]
        public void DetectMissingNamePartWhenFirstNameAndLastNameMissing()
        {
            // Assert
            this.clientDetailViewModel.HasMissingLastName.Should().BeTrue();
        }

        [Test]
        public void DetectMissingNamePartWhenLastNameMissing()
        {
            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(new Client() { FirstName = "Andre" });

            // Assert
            this.clientDetailViewModel.HasMissingLastName.Should().BeTrue();
        }

        [Test]
        public void DetectNotMissingNamePart()
        {
            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(new Client() { FirstName = "Andre", LastName = "Multerer" });

            // Assert
            this.clientDetailViewModel.HasMissingLastName.Should().BeFalse();
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
            this.clientDetailViewModel = new ClientDetailViewModel(client);

            // Assert
            this.clientDetailViewModel.ToString().Should().Be(ExpectedString);
        }

        #endregion
    }
}