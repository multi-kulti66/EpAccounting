// ///////////////////////////////////
// File: ArticlesOptionViewModelTest.cs
// Last Change: 02.11.2017  15:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class ArticlesOptionViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private ArticlesOptionViewModel _articlesOptionViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._articlesOptionViewModel = null;
            GC.Collect();
        }


        [Test]
        public void ArticleListInitializedAfterCreation()
        {
            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Should().NotBeNull();
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void ArticlesNotAddedAfterInitializationWhenNoConnection()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantity<Article>()).Throws<Exception>();

            // Act
            this._articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this._mockRepository.Object, this._mockDialogService.Object);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void ArticlesAddedAfterInitialization()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this._mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this._articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this._mockRepository.Object, this._mockDialogService.Object);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CommandsInitializedAfterCreation()
        {
            // Assert
            this._articlesOptionViewModel.Commands.Count.Should().Be(2);
            this._articlesOptionViewModel.StateCommands.Count.Should().Be(3);
        }

        [Test]
        public void NoArticleSelectedAfterInit()
        {
            // Assert
            this._articlesOptionViewModel.SelectedArticleViewModel.Should().BeNull();
        }

        [Test]
        public void IsNotEditableWhenInLoadedState()
        {
            // Assert
            this._articlesOptionViewModel.IsEditable.Should().BeFalse();
        }

        [Test]
        public void ChangesToEditMode()
        {
            // Act
            this._articlesOptionViewModel.ChangeToEditMode();

            // Assert
            this._articlesOptionViewModel.CurrentState.Should().BeOfType<ArticleEditState>();
        }

        [Test]
        public void IsEditableInEditMode()
        {
            // Act
            this._articlesOptionViewModel.ChangeToEditMode();

            // Arrange
            this._articlesOptionViewModel.IsEditable.Should().BeTrue();
        }

        [Test]
        public void ChangesToLoadedMode()
        {
            // Act
            this._articlesOptionViewModel.ChangeToEditMode();
            this._articlesOptionViewModel.ChangeToLoadedMode();

            // Assert
            this._articlesOptionViewModel.CurrentState.Should().BeOfType<ArticleLoadedState>();
        }

        [Test]
        public void CanNotAddItemWhenNoConnectionToDatabase()
        {
            // Arrange
            this._articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);

            // Act
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void CanNotAddItemWhenNotInEditMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void AddsItem()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);

            // Act
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CanSelectItem()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Act
            this._articlesOptionViewModel.SelectedArticleViewModel = this._articlesOptionViewModel.ArticleViewModels[0];

            // Assert
            this._articlesOptionViewModel.SelectedArticleViewModel.Should().NotBeNull();
        }

        [Test]
        public void RestoresArticles()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this._mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            this._articlesOptionViewModel.ChangeToEditMode();
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Act
            this._articlesOptionViewModel.RestoreArticles();

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CanNotDeleteItemWhenNotInEditMode()
        {
            // Assert
            this._articlesOptionViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotDeleteItemWhenNoItemSelected()
        {
            // Act
            this._articlesOptionViewModel.ChangeToEditMode();

            // Assert
            this._articlesOptionViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void DeletesItem()
        {
            // Arrange
            this._articlesOptionViewModel.ChangeToEditMode();
            this._articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);
            this._articlesOptionViewModel.SelectedArticleViewModel = this._articlesOptionViewModel.ArticleViewModels[0];

            // Act
            this._articlesOptionViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void DoesNotSaveChangesWhenArticleNumbersNotUnique()
        {
            // Act
            this._articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(ModelFactory.GetDefaultArticle(), this._mockRepository.Object, this._mockDialogService.Object));
            this._articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(ModelFactory.GetDefaultArticle(), this._mockRepository.Object, this._mockDialogService.Object));

            // Assert
            this._articlesOptionViewModel.SaveChanges().Should().BeFalse();
        }

        [Test]
        public void SavesChangesWhenArticleNumbersAreUnique()
        {
            // Assert
            this._articlesOptionViewModel.SaveChanges();
        }

        [Test]
        public void DeletesItemFromDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this._mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this._articlesOptionViewModel.SaveChanges();

            // Assert
            this._mockRepository.Verify(x => x.Delete(It.IsAny<Article>()), Times.Once());
        }

        [Test]
        public void OrdersArticlesAfterSave()
        {
            // Arrange
            this._articlesOptionViewModel.ChangeToEditMode();

            for (int i = 5; i > 0; i--)
            {
                Article article = new Article() { ArticleNumber = i };
                this._articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(article, this._mockRepository.Object, this._mockDialogService.Object));
            }

            // Act
            this._articlesOptionViewModel.SaveChanges();

            // Assert
            this._articlesOptionViewModel.ArticleViewModels.Should().BeInAscendingOrder(x => x.ArticleNumber);
        }

        [Test]
        public void SendsDisableWorkspaceMessageWhenInEditMode()
        {
            // Arrange
            NotificationMessage<bool> notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => notificationMessage = x);

            // Act
            this._articlesOptionViewModel.ChangeToEditMode();

            // Assert
            notificationMessage.Should().NotBeNull();
            notificationMessage.Notification.Should().Be(Resources.Message_WorkspaceEnableStateForMainVM);
            notificationMessage.Content.Should().BeFalse();
        }

        [Test]
        public void SendsEnableWorkspaceMessageWhenInLoadedMode()
        {
            // Arrange
            this._articlesOptionViewModel.ChangeToEditMode();
            NotificationMessage<bool> notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => notificationMessage = x);

            // Act
            this._articlesOptionViewModel.ChangeToLoadedMode();

            // Assert
            notificationMessage.Should().NotBeNull();
            notificationMessage.Notification.Should().Be(Resources.Message_WorkspaceEnableStateForMainVM);
            notificationMessage.Content.Should().BeTrue();
        }
    }
}