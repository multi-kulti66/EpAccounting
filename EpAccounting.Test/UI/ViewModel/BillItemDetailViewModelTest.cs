// ///////////////////////////////////
// File: BillItemDetailViewModelTest.cs
// Last Change: 21.06.2017  15:24
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemDetailViewModelTest
    {
        #region Test Methods

        [Test]
        public void CanReadVariableValues()
        {
            // Act
            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();

            // Assert
            billItemDetailViewModel.Position.Should().Be(ModelFactory.DefaultBillItemPosition);
            billItemDetailViewModel.ArticleNumber.Should().Be(ModelFactory.DefaultBillItemArticleNumber);
            billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultBillItemDescription);
            billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultBillItemAmount);
            billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultBillItemPrice);
            billItemDetailViewModel.Discount.Should().Be(ModelFactory.DefaultBillItemDiscount);
        }

        [Test]
        public void CorrectlyCalculatesSumWithoutDiscount()
        {
            // Arrange
            const int Amount = 2;
            const double Price = 4.99;
            const double Sum = 9.98;

            BillItem billItem = new BillItem();
            billItem.Amount = Amount;
            billItem.Price = Price;

            // Act
            BillItemDetailViewModel billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            // Assert
            billItemDetailViewModel.Sum.Should().Be(Sum);
        }

        [Test]
        public void CorrectoyCalculatesSumWithDiscount()
        {
            // Arrange
            const double Amount = 2;
            const double Price = 10;
            const double Discount = 10;
            const double Sum = 18;

            // Act
            BillItem billItem = new BillItem();
            billItem.Amount = Amount;
            billItem.Price = Price;
            billItem.Discount = Discount;

            BillItemDetailViewModel billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            // Assert
            billItemDetailViewModel.Sum.Should().Be(Sum);
        }

        [Test]
        public void CanModifyVariables()
        {
            // Arrange
            const int Position = 23;
            const int ArticleNumber = 44;
            const string Description = "Testdescription";
            const double Amount = 4;
            const double Price = 55;
            const double Discount = 50;

            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();

            // Act
            billItemDetailViewModel.Position = Position;
            billItemDetailViewModel.ArticleNumber = ArticleNumber;
            billItemDetailViewModel.Description = Description;
            billItemDetailViewModel.Amount = Amount;
            billItemDetailViewModel.Price = Price;
            billItemDetailViewModel.Discount = Discount;

            // Assert
            billItemDetailViewModel.Position.Should().Be(Position);
            billItemDetailViewModel.ArticleNumber.Should().Be(ArticleNumber);
            billItemDetailViewModel.Description.Should().Be(Description);
            billItemDetailViewModel.Amount.Should().Be(Amount);
            billItemDetailViewModel.Price.Should().Be(Price);
            billItemDetailViewModel.Discount.Should().Be(Discount);
        }

        [Test]
        public void RaisesProertyChangedWhenVariablesChanges()
        {
            // Arrange
            const int Position = 23;
            const int ArticleNumber = 44;
            const string Description = "Testdescription";
            const double Amount = 4;
            const double Price = 55;
            const double Discount = 50;

            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();
            billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billItemDetailViewModel.Position = Position;
            billItemDetailViewModel.ArticleNumber = ArticleNumber;
            billItemDetailViewModel.Description = Description;
            billItemDetailViewModel.Amount = Amount;
            billItemDetailViewModel.Price = Price;
            billItemDetailViewModel.Discount = Discount;

            // Assert
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Position);
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.ArticleNumber);
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Description);
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Amount);
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Price);
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Discount);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenAmountChanges()
        {
            // Arrange
            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();
            billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billItemDetailViewModel.Amount = 10;

            // Assert
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenPriceChanges()
        {
            // Arrange
            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();
            billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billItemDetailViewModel.Price = 10;

            // Assert
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenDiscountChanges()
        {
            // Arrange
            BillItemDetailViewModel billItemDetailViewModel = this.GetDefaultViewModel();
            billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billItemDetailViewModel.Discount = 50;

            // Assert
            billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        #endregion



        private BillItemDetailViewModel GetDefaultViewModel()
        {
            return new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem());
        }
    }
}