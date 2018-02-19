// ///////////////////////////////////
// File: ArticleViewModelTest.cs
// Last Change: 19.02.2018, 19:27
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
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class ArticleViewModelTest
    {
        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._articleViewModel = new ArticleViewModel(ModelFactory.GetDefaultArticle(),
                                                          this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._articleViewModel = null;
            GC.Collect();
        }

        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private ArticleViewModel _articleViewModel;

        [Test]
        public void PropertiesInitialized()
        {
            // Assert
            this._articleViewModel.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Count(prop => prop.DeclaringType == this._articleViewModel.GetType())
                .Should().Be(5);

            this._articleViewModel.Id.Should().Be(0);
            this._articleViewModel.Description.Should().Be(ModelFactory.DefaultArticleDescription);
            this._articleViewModel.Amount.Should().Be(ModelFactory.DefaultArticleAmount);
            this._articleViewModel.Price.Should().Be(ModelFactory.DefaultArticlePrice);
        }

        [Test]
        public void CanChangeProperties()
        {
            // Arrange
            const int newNumber = 5;
            const string newDescription = "Fernseher kaputt";
            const double newAmount = 5;
            const decimal newPrice = 10.50m;

            // Act
            this._articleViewModel.ArticleNumber = newNumber;
            this._articleViewModel.Description = newDescription;
            this._articleViewModel.Amount = newAmount;
            this._articleViewModel.Price = newPrice;

            // Assert
            this._articleViewModel.ArticleNumber.Should().Be(newNumber);
            this._articleViewModel.Description.Should().Be(newDescription);
            this._articleViewModel.Amount.Should().Be(newAmount);
            this._articleViewModel.Price.Should().Be(newPrice);
        }

        [Test]
        public void RaisesPropertyChangedWhenValuesChange()
        {
            // Arrange
            const int newNumber = 5;
            const string newDescription = "Fernseher kaputt";
            const double newAmount = 5;
            const decimal newPrice = 10.50m;
            this._articleViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._articleViewModel.ArticleNumber = newNumber;
            this._articleViewModel.Description = newDescription;
            this._articleViewModel.Amount = newAmount;
            this._articleViewModel.Price = newPrice;

            // Assert
            this._articleViewModel.ShouldRaisePropertyChangeFor(x => x.ArticleNumber);
            this._articleViewModel.ShouldRaisePropertyChangeFor(x => x.Description);
            this._articleViewModel.ShouldRaisePropertyChangeFor(x => x.Amount);
            this._articleViewModel.ShouldRaisePropertyChangeFor(x => x.Price);
        }

        [Test]
        public void SavesChanges()
        {
            // Act
            this._articleViewModel.Save();

            // Assert
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public void ShowMessageWhenClientCouldNotBeSaved()
        {
            // Arrange
            this._mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Article>())).Throws<Exception>();

            // Act
            this._articleViewModel.Save();

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}