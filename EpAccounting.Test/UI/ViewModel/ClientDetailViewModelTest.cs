// ///////////////////////////////////
// File: ClientDetailViewModelTest.cs
// Last Change: 08.12.2017  13:49
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
    using EpAccounting.UI.Properties;
    using ModelProperties = EpAccounting.Model.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class ClientDetailViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private ClientDetailViewModel _clientDetailViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._clientDetailViewModel = new ClientDetailViewModel(new Client { CityToPostalCode = new CityToPostalCode() }, this._mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._clientDetailViewModel = null;
            GC.Collect();
        }


        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientDetailViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void RaisesPropertyChangedWhenDetailsChange()
        {
            // Arrange
            this._clientDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientDetailViewModel.Id = 5;
            this._clientDetailViewModel.Title = ModelFactory.DefaultClientTitle;
            this._clientDetailViewModel.CompanyName = ModelFactory.DefaultClientCompanyName;
            this._clientDetailViewModel.FirstName = ModelFactory.DefaultClientFirstName;
            this._clientDetailViewModel.LastName = ModelFactory.DefaultClientLastName;
            this._clientDetailViewModel.Street = ModelFactory.DefaultClientStreet;
            this._clientDetailViewModel.HouseNumber = ModelFactory.DefaultClientHouseNumber;
            this._clientDetailViewModel.PostalCode = ModelFactory.DefaultCityToPostalCodePostalCode;
            this._clientDetailViewModel.City = ModelFactory.DefaultCityToPostalCodeCity;
            this._clientDetailViewModel.DateOfBirth = ModelFactory.DefaultClientDateOfBirth;
            this._clientDetailViewModel.MobileNumber = ModelFactory.DefaultClientMobileNumber;
            this._clientDetailViewModel.PhoneNumber1 = ModelFactory.DefaultClientPhoneNumber1;
            this._clientDetailViewModel.PhoneNumber2 = ModelFactory.DefaultClientPhoneNumber2;
            this._clientDetailViewModel.Telefax = ModelFactory.DefaultClientTelefax;
            this._clientDetailViewModel.Email = ModelFactory.DefaultClientEmail;

            // Assert
            this._clientDetailViewModel.Id.Should().Be(5);
            this._clientDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this._clientDetailViewModel.CompanyName.Should().Be(ModelFactory.DefaultClientCompanyName);
            this._clientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._clientDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this._clientDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this._clientDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this._clientDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultCityToPostalCodePostalCode);
            this._clientDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
            this._clientDetailViewModel.DateOfBirth.Should().Be(ModelFactory.DefaultClientDateOfBirth);
            this._clientDetailViewModel.MobileNumber.Should().Be(ModelFactory.DefaultClientMobileNumber);
            this._clientDetailViewModel.PhoneNumber1.Should().Be(ModelFactory.DefaultClientPhoneNumber1);
            this._clientDetailViewModel.PhoneNumber2.Should().Be(ModelFactory.DefaultClientPhoneNumber2);
            this._clientDetailViewModel.Telefax.Should().Be(ModelFactory.DefaultClientTelefax);
            this._clientDetailViewModel.Email.Should().Be(ModelFactory.DefaultClientEmail);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Id);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Title);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.CompanyName);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.FirstName);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.LastName);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Street);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.HouseNumber);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PostalCode);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.City);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.DateOfBirth);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.MobileNumber);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber1);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.PhoneNumber2);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Telefax);
            this._clientDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Email);
        }

        [Test]
        public void ReturnZeroWhenClientHasNoBills()
        {
            // Assert
            this._clientDetailViewModel.NumberOfBills.Should().Be(0);
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
            this._clientDetailViewModel = new ClientDetailViewModel(client, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.NumberOfBills.Should().Be(2);
        }

        [Test]
        public void ReturnZeroSalesWhenClientHasNoBills()
        {
            // Assert
            this._clientDetailViewModel.Sales.Should().Be(0);
        }

        [Test]
        public void ReturnSalesWhenClientHasBillsWithInklVat()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            client.AddBill(bill);

            // Act
            this._clientDetailViewModel = new ClientDetailViewModel(client, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.Sales.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void ReturnSalesWhenClientHasBillsWithZzglVat()
        {
            Client client = ModelFactory.GetDefaultClient();

            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.ZzglMwSt;
            client.AddBill(bill);

            // Act
            this._clientDetailViewModel = new ClientDetailViewModel(client, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.Sales.Should().BeApproximately(78.67m, 0.05m);
        }

        [Test]
        public void DetectMissingNamePartWhenFirstNameAndLastNameMissing()
        {
            // Assert
            this._clientDetailViewModel.HasMissingValues.Should().BeTrue();
        }

        [Test]
        public void DetectMissingNamePartWhenLastNameMissing()
        {
            // Act
            this._clientDetailViewModel = new ClientDetailViewModel(new Client { FirstName = "Andre", CityToPostalCode = new CityToPostalCode() }, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.HasMissingValues.Should().BeTrue();
        }

        [Test]
        public void DetectNotMissingValues()
        {
            // Act
            this._clientDetailViewModel = new ClientDetailViewModel(new Client { FirstName = "Andre", LastName = "Multerer", CityToPostalCode = new CityToPostalCode() { PostalCode = "94234" } }, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.HasMissingValues.Should().BeFalse();
        }

        [Test]
        public void GetClientDetailViewModelString()
        {
            // Arrange
            const int expectedId = 5;
            string expectedString = string.Format(ModelProperties.Resources.Client_ToString, expectedId,
                                                  ModelFactory.DefaultClientFirstName, ModelFactory.DefaultClientLastName,
                                                  ModelFactory.DefaultClientStreet, ModelFactory.DefaultClientHouseNumber,
                                                  ModelFactory.DefaultCityToPostalCodePostalCode, ModelFactory.DefaultCityToPostalCodeCity);

            // Act
            Client client = ModelFactory.GetDefaultClient();
            client.Id = expectedId;
            this._clientDetailViewModel = new ClientDetailViewModel(client, this._mockRepository.Object);

            // Assert
            this._clientDetailViewModel.ToString().Should().Be(expectedString);
        }

        [Test]
        public void FillsCityData()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<CityToPostalCode>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<CityToPostalCode> { ModelFactory.GetDefaultCityToPostalCode() });

            // Act
            this._clientDetailViewModel.PostalCode = "234";

            // Assert
            this._clientDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void DoesNotFillCityData()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<CityToPostalCode>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<CityToPostalCode>());

            // Act
            this._clientDetailViewModel.PostalCode = "234";

            // Assert
            this._clientDetailViewModel.City.Should().Be(null);
        }

        [Test]
        public void SendsUpdateCompanyEnableStateWhenTitleChanges()
        {
            // Arrange
            NotificationMessage message = null;
            Messenger.Default.Register<NotificationMessage>(this, x => message = x);

            // Act
            this._clientDetailViewModel.Title = ClientTitle.Frau;

            // Assert
            message.Should().NotBeNull();
            message.Notification.Should().Be(Resources.Message_UpdateCompanyNameEnableStateForClientEditVM);
        }

        [Test]
        public void ClearsCompanyNameWhenTitleNotACompany()
        {
            // Arrange
            this._clientDetailViewModel.Title = ClientTitle.Firma;
            this._clientDetailViewModel.CompanyName = ModelFactory.DefaultClientCompanyName;
            
            // Act
            this._clientDetailViewModel.Title = ClientTitle.Frau;

            // Assert
            this._clientDetailViewModel.CompanyName.Should().BeEmpty();
        }
    }
}