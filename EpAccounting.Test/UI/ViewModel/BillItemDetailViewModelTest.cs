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
        private Mock<IRepository> _mockRepository;
        private BillItemDetailViewModel _billItemDetailViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._billItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this._mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._billItemDetailViewModel = null;
            GC.Collect();
        }


        [Test]
        public void CanReadVariableValues()
        {
            // Assert
            this._billItemDetailViewModel.Id.Should().Be(0);
            this._billItemDetailViewModel.Position.Should().Be(ModelFactory.DefaultBillItemPosition);
            this._billItemDetailViewModel.ArticleNumber.Should().Be(ModelFactory.DefaultBillItemArticleNumber);
            this._billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultBillItemDescription);
            this._billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultBillItemAmount);
            this._billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultBillItemPrice);
            this._billItemDetailViewModel.Discount.Should().Be(ModelFactory.DefaultBillItemDiscount);
        }

        [Test]
        public void CorrectlyCalculatesSumWithoutDiscount()
        {
            // Arrange
            const int amount = 2;
            const decimal price = 4.99m;
            const decimal sum = 9.98m;

            BillItem billItem = new BillItem();
            billItem.Amount = amount;
            billItem.Price = price;

            // Act
            this._billItemDetailViewModel = new BillItemDetailViewModel(billItem, this._mockRepository.Object);

            // Assert
            this._billItemDetailViewModel.Sum.Should().Be(sum);
        }

        [Test]
        public void CorrectoyCalculatesSumWithDiscount()
        {
            // Arrange
            const double amount = 2;
            const decimal price = 10m;
            const double discount = 10;
            const decimal sum = 18m;

            // Act
            BillItem billItem = new BillItem();
            billItem.Amount = amount;
            billItem.Price = price;
            billItem.Discount = discount;

            this._billItemDetailViewModel = new BillItemDetailViewModel(billItem, this._mockRepository.Object);

            // Assert
            this._billItemDetailViewModel.Sum.Should().Be(sum);
        }

        [Test]
        public void CanModifyVariables()
        {
            // Arrange
            const int position = 23;
            const int articleNumber = 44;
            const string description = "Testdescription";
            const double amount = 4;
            const decimal price = 55m;
            const double discount = 50;

            // Act
            this._billItemDetailViewModel.Position = position;
            this._billItemDetailViewModel.ArticleNumber = articleNumber;
            this._billItemDetailViewModel.Description = description;
            this._billItemDetailViewModel.Amount = amount;
            this._billItemDetailViewModel.Price = price;
            this._billItemDetailViewModel.Discount = discount;

            // Assert
            this._billItemDetailViewModel.Position.Should().Be(position);
            this._billItemDetailViewModel.ArticleNumber.Should().Be(articleNumber);
            this._billItemDetailViewModel.Description.Should().Be(description);
            this._billItemDetailViewModel.Amount.Should().Be(amount);
            this._billItemDetailViewModel.Price.Should().Be(price);
            this._billItemDetailViewModel.Discount.Should().Be(discount);
        }

        [Test]
        public void RaisesProertyChangedWhenVariablesChanges()
        {
            // Arrange
            const int position = 23;
            const int articleNumber = 44;
            const string description = "Testdescription";
            const double amount = 4;
            const decimal price = 55;
            const double discount = 50;

            this._billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemDetailViewModel.Position = position;
            this._billItemDetailViewModel.ArticleNumber = articleNumber;
            this._billItemDetailViewModel.Description = description;
            this._billItemDetailViewModel.Amount = amount;
            this._billItemDetailViewModel.Price = price;
            this._billItemDetailViewModel.Discount = discount;

            // Assert
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Position);
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.ArticleNumber);
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Description);
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Amount);
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Price);
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Discount);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenAmountChanges()
        {
            // Arrange
            this._billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemDetailViewModel.Amount = 10;

            // Assert
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenPriceChanges()
        {
            // Arrange
            this._billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemDetailViewModel.Price = 10;

            // Assert
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void RaisePropertyChangedForSumWhenDiscountChanges()
        {
            // Arrange
            this._billItemDetailViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemDetailViewModel.Discount = 50;

            // Assert
            this._billItemDetailViewModel.ShouldRaisePropertyChangeFor(x => x.Sum);
        }

        [Test]
        public void FillsArticleDataWithAdjustedInklVatPrice()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();
            billItem.Bill = ModelFactory.GetDefaultBill();

            this._billItemDetailViewModel = new BillItemDetailViewModel(billItem, this._mockRepository.Object);
            this._mockRepository.Setup(x => x.GetByCriteria<Article>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this._billItemDetailViewModel.ArticleNumber = 3;

            // Assert
            this._billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this._billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this._billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice * (100 + (decimal)Settings.Default.VatPercentage) / 100);
        }

        [Test]
        public void FillsArticleDataWithZzglVatPrice()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();
            billItem.Bill = ModelFactory.GetDefaultBill();
            billItem.Bill.KindOfVat = KindOfVat.ZzglMwSt;

            this._billItemDetailViewModel = new BillItemDetailViewModel(billItem, this._mockRepository.Object);
            this._mockRepository.Setup(x => x.GetByCriteria<Article>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this._billItemDetailViewModel.ArticleNumber = 3;

            // Assert
            this._billItemDetailViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this._billItemDetailViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this._billItemDetailViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice);
        }
    }
}