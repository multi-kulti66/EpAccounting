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
        #region Fields

        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private Mock<ArticlesOptionViewModel> mockArticlesOptionViewModel;
        private ArticleEditState articleEditState;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.mockArticlesOptionViewModel = new Mock<ArticlesOptionViewModel>(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                                 this.mockRepository.Object, this.mockDialogService.Object);

            this.articleEditState = new ArticleEditState(this.mockArticlesOptionViewModel.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.mockArticlesOptionViewModel = null;
            this.articleEditState = null;
            GC.Collect();
        }

        #endregion



        [Test]
        public void CanNotSwitchToEditMode()
        {
            // Assert
            this.articleEditState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommitAndCancel()
        {
            // Assert
            this.articleEditState.CanCommit();
            this.articleEditState.CanCancel();
        }

        [Test]
        public void DoesNotCommitWhenSaveWasNotSuccessful()
        {
            // Arrange
            this.mockArticlesOptionViewModel.Setup(x => x.SaveChanges()).Returns(false);

            // Act
            this.articleEditState.Commit();

            // Assert
            this.mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Never);
        }

        [Test]
        public void CommitsWhenSaveWasNotSuccessful()
        {
            // Arrange
            this.mockArticlesOptionViewModel.Setup(x => x.SaveChanges()).Returns(true);

            // Act
            this.articleEditState.Commit();

            // Assert
            this.mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Once);
        }

        [Test]
        public void CancelsChanges()
        {
            // Act
            this.articleEditState.Cancel();

            // Assert
            this.mockArticlesOptionViewModel.Verify(x => x.RestoreArticles(), Times.Once);
            this.mockArticlesOptionViewModel.Verify(x => x.ChangeToLoadedMode(), Times.Once);
        }
    }
}