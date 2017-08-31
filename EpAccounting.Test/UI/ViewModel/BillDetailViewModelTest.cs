// ///////////////////////////////////
// File: BillDetailViewModelTest.cs
// Last Change: 22.08.2017  19:49
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillDetailViewModelTest
    {
        #region Fields

        private BillDetailViewModel billDetailViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.billDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill());
        }

        [TearDown]
        public void Cleanup()
        {
            this.billDetailViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void GetDefaultValues()
        {
            // Assert
            this.billDetailViewModel.BillId.Should().Be(0); // not in database
            this.billDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
            this.billDetailViewModel.KindOfVat.Should().Be(ModelFactory.DefaultBillKindOfVat);
            this.billDetailViewModel.VatPercentage.Should().Be(ModelFactory.DefaultBillVatPercentage);
            this.billDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            this.billDetailViewModel.ClientId.Should().Be(0); // not in database
            this.billDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this.billDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.billDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this.billDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this.billDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this.billDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultClientPostalCode);
            this.billDetailViewModel.City.Should().Be(ModelFactory.DefaultClientCity);
        }

        [Test]
        public void CanChangeValues()
        {
            // Arrange
            const int BillId = 4;
            const string KindOfBill = "Gutschein";
            const string KindOfVat = "zzgl";
            const double VatPercentage = 34.234;
            const string Date = "14.4.2012";
            const string Title = "Madam";
            const int ClientId = 10;
            const string FirstName = "Mia";
            const string LastName = "Meier";
            const string Street = "Benzstraße";
            const string HouseNumber = "234 a";
            const string PostalCode = "23512";
            const string City = "Berlin";

            // Act
            this.billDetailViewModel.BillId = BillId;
            this.billDetailViewModel.KindOfBill = KindOfBill;
            this.billDetailViewModel.KindOfVat = KindOfVat;
            this.billDetailViewModel.VatPercentage = VatPercentage;
            this.billDetailViewModel.Date = Date;
            this.billDetailViewModel.Title = Title;
            this.billDetailViewModel.ClientId = ClientId;
            this.billDetailViewModel.FirstName = FirstName;
            this.billDetailViewModel.LastName = LastName;
            this.billDetailViewModel.Street = Street;
            this.billDetailViewModel.HouseNumber = HouseNumber;
            this.billDetailViewModel.PostalCode = PostalCode;
            this.billDetailViewModel.City = City;

            // Assert
            this.billDetailViewModel.BillId.Should().Be(BillId);
            this.billDetailViewModel.KindOfBill.Should().Be(KindOfBill);
            this.billDetailViewModel.KindOfVat.Should().Be(KindOfVat);
            this.billDetailViewModel.VatPercentage.Should().Be(VatPercentage);
            this.billDetailViewModel.Date.Should().Be(Date);
            this.billDetailViewModel.Title.Should().Be(Title);
            this.billDetailViewModel.ClientId.Should().Be(ClientId);
            this.billDetailViewModel.FirstName.Should().Be(FirstName);
            this.billDetailViewModel.LastName.Should().Be(LastName);
            this.billDetailViewModel.Street.Should().Be(Street);
            this.billDetailViewModel.HouseNumber.Should().Be(HouseNumber);
            this.billDetailViewModel.PostalCode.Should().Be(PostalCode);
            this.billDetailViewModel.City.Should().Be(City);
        }

        #endregion
    }
}