// ///////////////////////////////////
// File: BillItemDetailViewModelTest.cs
// Last Change: 23.10.2017  21:10
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
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemDetailViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private BillItemDetailViewModel billItemDetailViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.billItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
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
            const decimal Price = 4.99m;
            const decimal Sum = 9.98m;

            BillItem billItem = new BillItem();
            billItem.Amount = Amount;
            billItem.Price = Price;

            // Act
            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem, this.mockRepository.Object);

            // Assert
            this.billItemDetailViewModel.Sum.Should().Be(Sum);
        }

        [Test]
        public void CorrectoyCalculatesSumWithDiscount()
        {
            // Arrange
            const double Amount = 2;
            const decimal Price = 10m;
            const double Discount = 10;
            const decimal Sum = 18m;

            // Act
            BillItem billItem = new BillItem();
            billItem.Amount = Amount;
            billItem.Price = Price;
            billItem.Discount = Discount;

            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem, this.mockRepository.Object);

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
            const decimal Price = 55m;
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
            const decimal Price = 55;
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

        [Test]
        public void FillsArticleDataWithAdjustedInklVatPrice()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();
            billItem.Bill = ModelFactory.GetDefaultBill();

            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem, this.mockRepository.Object);
            this.mockRepository.Setup(x => x.GetByCriteria<Article>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this.billItemDetailViewModel.ArticleNumber = 3;

            // Assert
            this.billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this.billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this.billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice * (100 + (decimal)Settings.Default.VatPercentage) / 100);
        }

        [Test]
        public void FillsArticleDataWithZzglVatPrice()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();
            billItem.Bill = ModelFactory.GetDefaultBill();
            billItem.Bill.KindOfVat = KindOfVat.zzgl_MwSt;

            this.billItemDetailViewModel = new BillItemDetailViewModel(billItem, this.mockRepository.Object);
            this.mockRepository.Setup(x => x.GetByCriteria<Article>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this.billItemDetailViewModel.ArticleNumber = 3;

            // Assert
            this.billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this.billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this.billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice);
        }

        #endregion
    }
}