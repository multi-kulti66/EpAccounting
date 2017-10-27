// ///////////////////////////////////
// File: ClientDetailViewModelTest.cs
// Last Change: 22.10.2017  15:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.Model.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class ClientDetailViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private ClientDetailViewModel clientDetailViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.clientDetailViewModel = new ClientDetailViewModel(new Client { CityToPostalCode = new CityToPostalCode() }, this.mockRepository.Object);
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
            this.clientDetailViewModel.Id = 5;
            this.clientDetailViewModel.Title = ModelFactory.DefaultClientTitle;
            this.clientDetailViewModel.FirstName = ModelFactory.DefaultClientFirstName;
            this.clientDetailViewModel.LastName = ModelFactory.DefaultClientLastName;
            this.clientDetailViewModel.Street = ModelFactory.DefaultClientStreet;
            this.clientDetailViewModel.HouseNumber = ModelFactory.DefaultClientHouseNumber;
            this.clientDetailViewModel.PostalCode = ModelFactory.DefaultCityToPostalCodePostalCode;
            this.clientDetailViewModel.City = ModelFactory.DefaultCityToPostalCodeCity;
            this.clientDetailViewModel.DateOfBirth = ModelFactory.DefaultClientDateOfBirth;
            this.clientDetailViewModel.MobileNumber = ModelFactory.DefaultClientMobileNumber;
            this.clientDetailViewModel.PhoneNumber1 = ModelFactory.DefaultClientPhoneNumber1;
            this.clientDetailViewModel.PhoneNumber2 = ModelFactory.DefaultClientPhoneNumber2;
            this.clientDetailViewModel.Telefax = ModelFactory.DefaultClientTelefax;
            this.clientDetailViewModel.Email = ModelFactory.DefaultClientEmail;

            // Assert
            this.clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Id);
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
            Client client = new Client { CityToPostalCode = new CityToPostalCode() };

            // Act
            client.AddBill(bill);
            client.AddBill(bill);
            this.clientDetailViewModel = new ClientDetailViewModel(client, this.mockRepository.Object);

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
        public void ReturnSalesWhenClientHasBillsWithInklVat()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            client.AddBill(bill);

            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(client, this.mockRepository.Object);

            // Assert
            this.clientDetailViewModel.Sales.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void ReturnSalesWhenClientHasBillsWithZzglVat()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.zzgl_MwSt;
            client.AddBill(bill);

            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(client, this.mockRepository.Object);

            // Assert
            this.clientDetailViewModel.Sales.Should().BeApproximately(78.67m, 0.05m);
        }

        [Test]
        public void DetectMissingNamePartWhenFirstNameAndLastNameMissing()
        {
            // Assert
            this.clientDetailViewModel.HasMissingValues.Should().BeTrue();
        }

        [Test]
        public void DetectMissingNamePartWhenLastNameMissing()
        {
            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(new Client { FirstName = "Andre", CityToPostalCode = new CityToPostalCode() }, this.mockRepository.Object);

            // Assert
            this.clientDetailViewModel.HasMissingValues.Should().BeTrue();
        }

        [Test]
        public void DetectNotMissingValues()
        {
            // Act
            this.clientDetailViewModel = new ClientDetailViewModel(new Client { FirstName = "Andre", LastName = "Multerer", CityToPostalCode = new CityToPostalCode() { PostalCode = "94234" } }, this.mockRepository.Object);

            // Assert
            this.clientDetailViewModel.HasMissingValues.Should().BeFalse();
        }

        [Test]
        public void GetClientDetailViewModelString()
        {
            // Arrange
            const int ExpectedId = 5;
            string ExpectedString = string.Format(Resources.Client_ToString, ExpectedId,
                                                  ModelFactory.DefaultClientFirstName, ModelFactory.DefaultClientLastName,
                                                  ModelFactory.DefaultClientStreet, ModelFactory.DefaultClientHouseNumber,
                                                  ModelFactory.DefaultCityToPostalCodePostalCode, ModelFactory.DefaultCityToPostalCodeCity);

            // Act
            Client client = ModelFactory.GetDefaultClient();
            client.Id = ExpectedId;
            this.clientDetailViewModel = new ClientDetailViewModel(client, this.mockRepository.Object);

            // Assert
            this.clientDetailViewModel.ToString().Should().Be(ExpectedString);
        }

        [Test]
        public void FillsCityData()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<CityToPostalCode>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<CityToPostalCode> { ModelFactory.GetDefaultCityToPostalCode() });

            // Act
            this.clientDetailViewModel.PostalCode = "234";

            // Assert
            this.clientDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void DoesNotFillCityData()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<CityToPostalCode>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<CityToPostalCode>());

            // Act
            this.clientDetailViewModel.PostalCode = "234";

            // Assert
            this.clientDetailViewModel.City.Should().Be(null);
        }

        #endregion
    }
}