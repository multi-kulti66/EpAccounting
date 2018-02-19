// ///////////////////////////////////
// File: ArticleEditStateTest.cs
// Last Change: 17.09.2017  16:50
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using System;
    using EpAccounting.Business;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class ArticleEditStateTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private Mock<ArticlesOptionViewModel> _mockArticlesOptionViewModel;
        private ArticleEditState _articleEditState;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._mockArticlesOptionViewModel = new Mock<ArticlesOptionViewModel>(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                                 this._mockRepository.Object, this._mockDialogService.Object);

            this._articleEditState = new ArticleEditState(this._mockArticlesOptionViewModel.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._mockArticlesOptionViewModel = null;
            this._articleEditState = null;
            GC.Collect();
        }


        [Test]
        public void CanNotSwitchToEditMode()
        {
            // Assert
            this._articleEditState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommitAndCancel()
        {
            // Assert
            this._articleEditState.CanCommit();
            this._articleEditState.CanCancel();
        }

        [Test]
        public void DoesNotCommitWhenSaveWasNotSuccessful()
        {
            // Arrange
            this._mockArticlesOptionViewModel.Setup(x => x.SaveChanges()).Returns(false);

            // Act
            this._articleEditState.Commit();

            // Assert
            this._mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Never);
        }

        [Test]
        public void CommitsWhenSaveWasNotSuccessful()
        {
            // Arrange
            this._mockArticlesOptionViewModel.Setup(x => x.SaveChanges()).Returns(true);

            // Act
            this._articleEditState.Commit();

            // Assert
            this._mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Once);
        }

        [Test]
        public void CancelsChanges()
        {
            // Act
            this._articleEditState.Cancel();

            // Assert
            this._mockArticlesOptionViewModel.Verify(x => x.RestoreArticles(), Times.Once);
            this._mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Once);
        }
    }
}