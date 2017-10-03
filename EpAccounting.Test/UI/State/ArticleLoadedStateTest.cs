// ///////////////////////////////////
// File: ArticleLoadedStateTest.cs
// Last Change: 17.09.2017  16:49
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
    public class ArticleLoadedStateTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private Mock<ArticlesOptionViewModel> mockArticlesOptionViewModel;
        private ArticleLoadedState articleLoadedState;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.mockArticlesOptionViewModel = new Mock<ArticlesOptionViewModel>(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                                 this.mockRepository.Object, this.mockDialogService.Object);

            this.articleLoadedState = new ArticleLoadedState(this.mockArticlesOptionViewModel.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.mockArticlesOptionViewModel = null;
            this.articleLoadedState = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void CanSwitchToEditMode()
        {
            // Assert
            this.articleLoadedState.CanSwitchToEditMode().Should().BeTrue();
        }

        [Test]
        public void CanNotCommitOrCancel()
        {
            // Assert
            this.articleLoadedState.CanCommit().Should().BeFalse();
            this.articleLoadedState.CanCancel().Should().BeFalse();
        }

        [Test]
        public void ChangesToEditMode()
        {
            // Act
            this.articleLoadedState.SwitchToEditMode();

            // Assert
            this.mockArticlesOptionViewModel.Verify(x => x.ChangeToEditMode(), Times.Once);
        }

        #endregion
    }
}