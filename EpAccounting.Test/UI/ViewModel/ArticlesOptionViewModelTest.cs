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
        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private ArticlesOptionViewModel articlesOptionViewModel;


        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.articlesOptionViewModel = null;
            GC.Collect();
        }


        [Test]
        public void ArticleListInitializedAfterCreation()
        {
            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Should().NotBeNull();
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void ArticlesNotAddedAfterInitializationWhenNoConnection()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetQuantity<Article>()).Throws<Exception>();

            // Act
            this.articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this.mockRepository.Object, this.mockDialogService.Object);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void ArticlesAddedAfterInitialization()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this.mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this.articlesOptionViewModel = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                       this.mockRepository.Object, this.mockDialogService.Object);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CommandsInitializedAfterCreation()
        {
            // Assert
            this.articlesOptionViewModel.Commands.Count.Should().Be(2);
            this.articlesOptionViewModel.StateCommands.Count.Should().Be(3);
        }

        [Test]
        public void NoArticleSelectedAfterInit()
        {
            // Assert
            this.articlesOptionViewModel.SelectedArticleViewModel.Should().BeNull();
        }

        [Test]
        public void IsNotEditableWhenInLoadedState()
        {
            // Assert
            this.articlesOptionViewModel.IsEditable.Should().BeFalse();
        }

        [Test]
        public void ChangesToEditMode()
        {
            // Act
            this.articlesOptionViewModel.ChangeToEditMode();

            // Assert
            this.articlesOptionViewModel.CurrentState.Should().BeOfType<ArticleEditState>();
        }

        [Test]
        public void IsEditableInEditMode()
        {
            // Act
            this.articlesOptionViewModel.ChangeToEditMode();

            // Arrange
            this.articlesOptionViewModel.IsEditable.Should().BeTrue();
        }

        [Test]
        public void ChangesToLoadedMode()
        {
            // Act
            this.articlesOptionViewModel.ChangeToEditMode();
            this.articlesOptionViewModel.ChangeToLoadedMode();

            // Assert
            this.articlesOptionViewModel.CurrentState.Should().BeOfType<ArticleLoadedState>();
        }

        [Test]
        public void CanNotAddItemWhenNoConnectionToDatabase()
        {
            // Arrange
            this.articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);

            // Act
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void CanNotAddItemWhenNotInEditMode()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void AddsItem()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);
            this.articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);

            // Act
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CanSelectItem()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);
            this.articlesOptionViewModel.SwitchToEditModeCommand.RelayCommand.Execute(null);
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Act
            this.articlesOptionViewModel.SelectedArticleViewModel = this.articlesOptionViewModel.ArticleViewModels[0];

            // Assert
            this.articlesOptionViewModel.SelectedArticleViewModel.Should().NotBeNull();
        }

        [Test]
        public void RestoresArticles()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this.mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            this.articlesOptionViewModel.ChangeToEditMode();
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Act
            this.articlesOptionViewModel.RestoreArticles();

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(1);
        }

        [Test]
        public void CanNotDeleteItemWhenNotInEditMode()
        {
            // Assert
            this.articlesOptionViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotDeleteItemWhenNoItemSelected()
        {
            // Act
            this.articlesOptionViewModel.ChangeToEditMode();

            // Assert
            this.articlesOptionViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void DeletesItem()
        {
            // Arrange
            this.articlesOptionViewModel.ChangeToEditMode();
            this.articlesOptionViewModel.AddItemCommand.RelayCommand.Execute(null);
            this.articlesOptionViewModel.SelectedArticleViewModel = this.articlesOptionViewModel.ArticleViewModels[0];

            // Act
            this.articlesOptionViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Count.Should().Be(0);
        }

        [Test]
        public void DoesNotSaveChangesWhenArticleNumbersNotUnique()
        {
            // Act
            this.articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(ModelFactory.GetDefaultArticle(), this.mockRepository.Object));
            this.articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(ModelFactory.GetDefaultArticle(), this.mockRepository.Object));

            // Assert
            this.articlesOptionViewModel.SaveChanges().Should().BeFalse();
        }

        [Test]
        public void SavesChangesWhenArticleNumbersAreUnique()
        {
            // Assert
            this.articlesOptionViewModel.SaveChanges();
        }

        [Test]
        public void DeletesItemFromDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetQuantity<Article>()).Returns(1);
            this.mockRepository.Setup(x => x.GetAll<Article>(It.IsAny<int>()))
                .Returns(new List<Article> { ModelFactory.GetDefaultArticle() });

            // Act
            this.articlesOptionViewModel.SaveChanges();

            // Assert
            this.mockRepository.Verify(x => x.Delete(It.IsAny<Article>()), Times.Once());
        }

        [Test]
        public void OrdersArticlesAfterSave()
        {
            // Arrange
            this.articlesOptionViewModel.ChangeToEditMode();

            for (int i = 5; i > 0; i--)
            {
                Article article = new Article() { ArticleNumber = i };
                this.articlesOptionViewModel.ArticleViewModels.Add(new ArticleViewModel(article, this.mockRepository.Object));
            }

            // Act
            this.articlesOptionViewModel.SaveChanges();

            // Assert
            this.articlesOptionViewModel.ArticleViewModels.Should().BeInAscendingOrder(x => x.ArticleNumber);
        }

        [Test]
        public void SendsDisableWorkspaceMessageWhenInEditMode()
        {
            // Arrange
            NotificationMessage<bool> notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => notificationMessage = x);

            // Act
            this.articlesOptionViewModel.ChangeToEditMode();

            // Assert
            notificationMessage.Should().NotBeNull();
            notificationMessage.Notification.Should().Be(Resources.Message_WorkspaceEnableStateForMainVM);
            notificationMessage.Content.Should().BeFalse();
        }

        [Test]
        public void SendsEnableWorkspaceMessageWhenInLoadedMode()
        {
            // Arrange
            this.articlesOptionViewModel.ChangeToEditMode();
            NotificationMessage<bool> notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => notificationMessage = x);

            // Act
            this.articlesOptionViewModel.ChangeToLoadedMode();

            // Assert
            notificationMessage.Should().NotBeNull();
            notificationMessage.Notification.Should().Be(Resources.Message_WorkspaceEnableStateForMainVM);
            notificationMessage.Content.Should().BeTrue();
        }
    }
}