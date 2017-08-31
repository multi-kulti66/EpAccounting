// ///////////////////////////////////
// File: BillItemDetailViewModelTest.cs
// Last Change: 22.08.2017  20:54
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemDetailViewModelTest
    {
        #region Fields

        private BillItemDetailViewModel billItemDetailViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.billItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem());
        }

        [TearDown]
        public void Cleanup()
        {
            this.billItemDetailViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void CanReadVariableValues()
        {
            // Assert
            this.billItemDetailViewModel.Id.Should().Be(0);
            this.billItemDetailViewModel.Position.Should().Be(ModelFactory.DefaultBillItemPosition);
            this.billItemDetailViewModel.ArticleNumber.Should().Be(ModelFactory.DefaultBillItemArticleNumber);
            this.billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultBillItemDescription);
            this.billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultBillItemAmount);
            this.billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultBillItemPrice);
            this.billItemDetailViewModel.Discount.Should().Be(ModelFactory.DefaultBillItemDiscount);
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
            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            // Assert
            this.billItemDetailViewModel.Sum.Should().Be(Sum);
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

            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            // Assert
            this.billItemDetailViewModel.Sum.Should().Be(Sum);
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

            // Act
            this.billItemDetailViewModel.Position = Position;
            this.billItemDetailViewModel.ArticleNumber = ArticleNumber;
            this.billItemDetailViewModel.Description = Description;
            this.billItemDetailViewModel.Amount = Amount;
            this.billItemDetailViewModel.Price = Price;
            this.billItemDetailViewModel.Discount = Discount;

            // Assert
            this.billItemDetailViewModel.Position.Should().Be(Position);
            this.billItemDetailViewModel.ArticleNumber.Should().Be(ArticleNumber);
            this.billItemDetailViewModel.Description.Should().Be(Description);
            this.billItemDetailViewModel.Amount.Should().Be(Amount);
            this.billItemDetailViewModel.Price.Should().Be(Price);
            this.billItemDetailViewModel.Discount.Should().Be(Discount);
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

            this.billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billItemDetailViewModel.Position = Position;
            this.billItemDetailViewModel.ArticleNumber = ArticleNumber;
            this.billItemDetailViewModel.Description = Description;
            this.billItemDetailViewModel.Amount = Amount;
            this.billItemDetailViewModel.Price = Price;
            this.billItemDetailViewModel.Discount = Discount;

            // Assert
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Position);
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.ArticleNumber);
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Description);
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Amount);
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Price);
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Discount);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenAmountChanges()
        {
            // Arrange
            this.billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billItemDetailViewModel.Amount = 10;

            // Assert
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenPriceChanges()
        {
            // Arrange
            this.billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billItemDetailViewModel.Price = 10;

            // Assert
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenDiscountChanges()
        {
            // Arrange
            this.billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billItemDetailViewModel.Discount = 50;

            // Assert
            this.billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        #endregion
    }
}