// ///////////////////////////////////
// File: BillDetailViewModelTest.cs
// Last Change: 14.09.2017  19:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Castle.Core.Smtp;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillDetailViewModelTest
    {
        private Mock<IRepository> mockRepository;
        private BillDetailViewModel billDetailViewModel;


        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.billDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.billDetailViewModel = null;
            GC.Collect();
        }


        [Test]
        public void GetDefaultValues()
        {
            // Assert
            this.billDetailViewModel.Id.Should().Be(0); // not in database
            this.billDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
            this.billDetailViewModel.KindOfVat.Should().Be(ModelFactory.DefaultBillKindOfVat);
            this.billDetailViewModel.VatPercentage.Should().Be(ModelFactory.DefaultBillVatPercentage);
            this.billDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            this.billDetailViewModel.ClientId.Should().Be(0); // not in database
            this.billDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this.billDetailViewModel.CompanyName.Should().Be(ModelFactory.DefaultClientCompanyName);
            this.billDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.billDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this.billDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this.billDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this.billDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultCityToPostalCodePostalCode);
            this.billDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void CanChangeValues()
        {
            // Arrange
            const int BillId = 4;
            KindOfBill KindOfBill = KindOfBill.Gutschrift;
            KindOfVat KindOfVat = KindOfVat.zzgl_MwSt;
            const double VatPercentage = 34.234;
            const string Date = "14.04.2014";
            ClientTitle Title = ClientTitle.Frau;
            const int ClientId = 10;
            const string CompanyName = "hololulu";
            const string FirstName = "Mia";
            const string LastName = "Meier";
            const string Street = "Benzstraße";
            const string HouseNumber = "234 a";
            const string PostalCode = "23512";
            const string City = "Berlin";

            // Act
            this.billDetailViewModel.Id = BillId;
            this.billDetailViewModel.KindOfBill = KindOfBill;
            this.billDetailViewModel.KindOfVat = KindOfVat;
            this.billDetailViewModel.VatPercentage = VatPercentage;
            this.billDetailViewModel.Date = Date;
            this.billDetailViewModel.ClientId = ClientId;
            this.billDetailViewModel.Title = Title;
            this.billDetailViewModel.CompanyName = CompanyName;
            this.billDetailViewModel.FirstName = FirstName;
            this.billDetailViewModel.LastName = LastName;
            this.billDetailViewModel.Street = Street;
            this.billDetailViewModel.HouseNumber = HouseNumber;
            this.billDetailViewModel.PostalCode = PostalCode;
            this.billDetailViewModel.City = City;

            // Assert
            this.billDetailViewModel.Id.Should().Be(BillId);
            this.billDetailViewModel.KindOfBill.Should().Be(KindOfBill);
            this.billDetailViewModel.KindOfVat.Should().Be(KindOfVat);
            this.billDetailViewModel.VatPercentage.Should().Be(VatPercentage);
            this.billDetailViewModel.Date.Should().Be(Date);
            this.billDetailViewModel.ClientId.Should().Be(ClientId);
            this.billDetailViewModel.Title.Should().Be(Title);
            this.billDetailViewModel.CompanyName.Should().Be(CompanyName);
            this.billDetailViewModel.FirstName.Should().Be(FirstName);
            this.billDetailViewModel.LastName.Should().Be(LastName);
            this.billDetailViewModel.Street.Should().Be(Street);
            this.billDetailViewModel.HouseNumber.Should().Be(HouseNumber);
            this.billDetailViewModel.PostalCode.Should().Be(PostalCode);
            this.billDetailViewModel.City.Should().Be(City);
        }

        [Test]
        public void SendsUpdateMessageWhenKindOfVatChanges()
        {
            // Arrange
            NotificationMessage notificaton = null;
            Messenger.Default.Register<NotificationMessage>(this, x => notificaton = x);

            // Act
            this.billDetailViewModel.KindOfVat = KindOfVat.without_MwSt;

            // Assert
            notificaton.Should().NotBeNull();
            notificaton.Notification.Should().Be(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM);
        }

        [Test]
        public void ReloadsBillWhenPrintedMessageReceived()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill());
            List<string> propertyChangedList = new List<string>();
            this.billDetailViewModel.MonitorEvents<INotifyPropertyChanged>();
            this.billDetailViewModel.PropertyChanged += (sender, e) => propertyChangedList.Add(e.PropertyName);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));

            // Assert
            propertyChangedList.Count.Should().Be(15);
            this.mockRepository.Verify(x => x.GetById<Bill>(0), Times.Once);
        }

        [Test]
        public void DoesNotReloadBillWhenPrintedMessageReceivedWithDifferentId()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill());
            List<string> propertyChangedList = new List<string>();
            this.billDetailViewModel.MonitorEvents<INotifyPropertyChanged>();
            this.billDetailViewModel.PropertyChanged += (sender, e) => propertyChangedList.Add(e.PropertyName);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));

            // Assert
            propertyChangedList.Count.Should().Be(0);
            this.mockRepository.Verify(x => x.GetById<Bill>(It.IsAny<int>()), Times.Never());
        }
    }
}