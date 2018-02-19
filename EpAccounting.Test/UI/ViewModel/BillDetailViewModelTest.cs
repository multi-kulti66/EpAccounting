// ///////////////////////////////////
// File: BillDetailViewModelTest.cs
// Last Change: 19.02.2018, 19:33
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
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class BillDetailViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private BillDetailViewModel _billDetailViewModel;



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._billDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._billDetailViewModel = null;
            GC.Collect();
        }

        #endregion



        [Test]
        public void GetDefaultValues()
        {
            // Assert
            this._billDetailViewModel.Id.Should().Be(0); // not in database
            this._billDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
            this._billDetailViewModel.KindOfVat.Should().Be(ModelFactory.DefaultBillKindOfVat);
            this._billDetailViewModel.VatPercentage.Should().Be(ModelFactory.DefaultBillVatPercentage);
            this._billDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            this._billDetailViewModel.ClientId.Should().Be(0); // not in database
            this._billDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this._billDetailViewModel.CompanyName.Should().Be(ModelFactory.DefaultClientCompanyName);
            this._billDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._billDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this._billDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this._billDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this._billDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultCityToPostalCodePostalCode);
            this._billDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void CanChangeValues()
        {
            // Arrange
            const int billId = 4;
            KindOfBill kindOfBill = KindOfBill.Gutschrift;
            KindOfVat kindOfVat = KindOfVat.ZzglMwSt;
            const double vatPercentage = 34.234;
            const string date = "14.04.2014";
            ClientTitle title = ClientTitle.Frau;
            const int clientId = 10;
            const string companyName = "hololulu";
            const string firstName = "Mia";
            const string lastName = "Meier";
            const string street = "Benzstraße";
            const string houseNumber = "234 a";
            const string postalCode = "23512";
            const string city = "Berlin";

            // Act
            this._billDetailViewModel.Id = billId;
            this._billDetailViewModel.KindOfBill = kindOfBill;
            this._billDetailViewModel.KindOfVat = kindOfVat;
            this._billDetailViewModel.VatPercentage = vatPercentage;
            this._billDetailViewModel.Date = date;
            this._billDetailViewModel.ClientId = clientId;
            this._billDetailViewModel.Title = title;
            this._billDetailViewModel.CompanyName = companyName;
            this._billDetailViewModel.FirstName = firstName;
            this._billDetailViewModel.LastName = lastName;
            this._billDetailViewModel.Street = street;
            this._billDetailViewModel.HouseNumber = houseNumber;
            this._billDetailViewModel.PostalCode = postalCode;
            this._billDetailViewModel.City = city;

            // Assert
            this._billDetailViewModel.Id.Should().Be(billId);
            this._billDetailViewModel.KindOfBill.Should().Be(kindOfBill);
            this._billDetailViewModel.KindOfVat.Should().Be(kindOfVat);
            this._billDetailViewModel.VatPercentage.Should().Be(vatPercentage);
            this._billDetailViewModel.Date.Should().Be(date);
            this._billDetailViewModel.ClientId.Should().Be(clientId);
            this._billDetailViewModel.Title.Should().Be(title);
            this._billDetailViewModel.CompanyName.Should().Be(companyName);
            this._billDetailViewModel.FirstName.Should().Be(firstName);
            this._billDetailViewModel.LastName.Should().Be(lastName);
            this._billDetailViewModel.Street.Should().Be(street);
            this._billDetailViewModel.HouseNumber.Should().Be(houseNumber);
            this._billDetailViewModel.PostalCode.Should().Be(postalCode);
            this._billDetailViewModel.City.Should().Be(city);
        }

        [Test]
        public void SendsUpdateMessageWhenKindOfVatChanges()
        {
            // Arrange
            NotificationMessage notificaton = null;
            Messenger.Default.Register<NotificationMessage>(this, x => notificaton = x);

            // Act
            this._billDetailViewModel.KindOfVat = KindOfVat.WithoutMwSt;

            // Assert
            notificaton.Should().NotBeNull();
            notificaton.Notification.Should().Be(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM);
        }

        [Test]
        public void ReloadsBillWhenPrintedMessageReceived()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill());
            List<string> propertyChangedList = new List<string>();
            this._billDetailViewModel.MonitorEvents<INotifyPropertyChanged>();
            this._billDetailViewModel.PropertyChanged += (sender, e) => propertyChangedList.Add(e.PropertyName);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));

            // Assert
            propertyChangedList.Count.Should().Be(15);
            this._mockRepository.Verify(x => x.GetById<Bill>(0), Times.Once);
        }

        [Test]
        public void ShowMessageWhenBillCouldNotBeReloaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Throws<Exception>();

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));

            // Assert
            this._mockRepository.Verify(x => x.GetById<Bill>(0), Times.Once);
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoesNotReloadBillWhenPrintedMessageReceivedWithDifferentId()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill());
            List<string> propertyChangedList = new List<string>();
            this._billDetailViewModel.MonitorEvents<INotifyPropertyChanged>();
            this._billDetailViewModel.PropertyChanged += (sender, e) => propertyChangedList.Add(e.PropertyName);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));

            // Assert
            propertyChangedList.Count.Should().Be(0);
            this._mockRepository.Verify(x => x.GetById<Bill>(It.IsAny<int>()), Times.Never());
        }
    }
}