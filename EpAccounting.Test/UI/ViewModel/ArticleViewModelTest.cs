// ///////////////////////////////////
// File: ArticleViewModelTest.cs
// Last Change: 18.09.2017  20:45
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class ArticleViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private ArticleViewModel articleViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.articleViewModel = new ArticleViewModel(ModelFactory.GetDefaultArticle(),
                                                         this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.articleViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void PropertiesInitialized()
        {
            // Assert
            this.articleViewModel.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Count(prop => prop.DeclaringType == this.articleViewModel.GetType())
                .Should().Be(5);

            this.articleViewModel.Id.Should().Be(0);
            this.articleViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this.articleViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this.articleViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice);
        }

        [Test]
        public void CanChangeProperties()
        {
            // Arrange
            const int NewNumber = 5;
            const string NewDescription = "Fernseher kaputt";
            const double NewAmount = 5;
            const decimal NewPrice = 10.50m;

            // Act
            this.articleViewModel.ArticleNumber = NewNumber;
            this.articleViewModel.Description = NewDescription;
            this.articleViewModel.Amount = NewAmount;
            this.articleViewModel.Price = NewPrice;

            // Assert
            this.articleViewModel.ArticleNumber.Should().Be(NewNumber);
            this.articleViewModel.Description.Should().Be(NewDescription);
            this.articleViewModel.Amount.Should().Be(NewAmount);
            this.articleViewModel.Price.Should().Be(NewPrice);
        }

        [Test]
        public void RaisesPropertyChangedWhenValuesChange()
        {
            // Arrange
            const int NewNumber = 5;
            const string NewDescription = "Fernseher kaputt";
            const double NewAmount = 5;
            const decimal NewPrice = 10.50m;
            this.articleViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.articleViewModel.ArticleNumber = NewNumber;
            this.articleViewModel.Description = NewDescription;
            this.articleViewModel.Amount = NewAmount;
            this.articleViewModel.Price = NewPrice;

            // Assert
            this.articleViewModel.ShouldRaisePropertyChangeFor(x => x.ArticleNumber);
            this.articleViewModel.ShouldRaisePropertyChangeFor(x => x.Description);
            this.articleViewModel.ShouldRaisePropertyChangeFor(x => x.Amount);
            this.articleViewModel.ShouldRaisePropertyChangeFor(x => x.Price);
        }

        [Test]
        public void SavesChanges()
        {
            // Act
            this.articleViewModel.Save();

            // Assert
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Article>()), Times.Once);
        }

        #endregion
    }
}