// ///////////////////////////////////
// File: BillDetailViewModelTest.cs
// Last Change: 09.06.2017  20:04
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using EpAccounting.Test.Model;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillDetailViewModelTest
    {
        #region Test Methods

        [Test]
        public void GetDefaultValues()
        {
            // Act
            BillDetailViewModel billDetailViewModel = this.GetDefaultViewModel();

            // Assert
            billDetailViewModel.BillId.Should().Be(0); // not in database
            billDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
            billDetailViewModel.KindOfVat.Should().Be(ModelFactory.DefaultBillKindOfVat);
            billDetailViewModel.VatPercentage.Should().Be(ModelFactory.DefaultBillVatPercentage);
            billDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            billDetailViewModel.ClientId.Should().Be(0); // not in database
            billDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            billDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            billDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            billDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            billDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            billDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultClientPostalCode);
            billDetailViewModel.City.Should().Be(ModelFactory.DefaultClientCity);
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
            BillDetailViewModel billDetailViewModel = this.GetDefaultViewModel();

            // Act
            billDetailViewModel.BillId = BillId;
            billDetailViewModel.KindOfBill = KindOfBill;
            billDetailViewModel.KindOfVat = KindOfVat;
            billDetailViewModel.VatPercentage = VatPercentage;
            billDetailViewModel.Date = Date;
            billDetailViewModel.Title = Title;
            billDetailViewModel.ClientId = ClientId;
            billDetailViewModel.FirstName = FirstName;
            billDetailViewModel.LastName = LastName;
            billDetailViewModel.Street = Street;
            billDetailViewModel.HouseNumber = HouseNumber;
            billDetailViewModel.PostalCode = PostalCode;
            billDetailViewModel.City = City;

            // Assert
            billDetailViewModel.BillId.Should().Be(BillId);
            billDetailViewModel.KindOfBill.Should().Be(KindOfBill);
            billDetailViewModel.KindOfVat.Should().Be(KindOfVat);
            billDetailViewModel.VatPercentage.Should().Be(VatPercentage);
            billDetailViewModel.Date.Should().Be(Date);
            billDetailViewModel.Title.Should().Be(Title);
            billDetailViewModel.ClientId.Should().Be(ClientId);
            billDetailViewModel.FirstName.Should().Be(FirstName);
            billDetailViewModel.LastName.Should().Be(LastName);
            billDetailViewModel.Street.Should().Be(Street);
            billDetailViewModel.HouseNumber.Should().Be(HouseNumber);
            billDetailViewModel.PostalCode.Should().Be(PostalCode);
            billDetailViewModel.City.Should().Be(City);
        }

        #endregion



        private BillDetailViewModel GetDefaultViewModel()
        {
            return new BillDetailViewModel(ModelFactory.GetDefaultBill());
        }
    }
}