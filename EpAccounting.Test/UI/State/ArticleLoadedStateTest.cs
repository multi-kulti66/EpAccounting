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
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private Mock<ArticlesOptionViewModel> _mockArticlesOptionViewModel;
        private ArticleLoadedState _articleLoadedState;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._mockArticlesOptionViewModel = new Mock<ArticlesOptionViewModel>(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                                 this._mockRepository.Object, this._mockDialogService.Object);

            this._articleLoadedState = new ArticleLoadedState(this._mockArticlesOptionViewModel.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._mockArticlesOptionViewModel = null;
            this._articleLoadedState = null;
            GC.Collect();
        }


        [Test]
        public void CanSwitchToEditMode()
        {
            // Assert
            this._articleLoadedState.CanSwitchToEditMode().Should().BeTrue();
        }

        [Test]
        public void CanNotCommitOrCancel()
        {
            // Assert
            this._articleLoadedState.CanCommit().Should().BeFalse();
            this._articleLoadedState.CanCancel().Should().BeFalse();
        }

        [Test]
        public void ChangesToEditMode()
        {
            // Act
            this._articleLoadedState.SwitchToEditMode();

            // Assert
            this._mockArticlesOptionViewModel.Verify(x => x.ChangeToEditMode(), Times.Once);
        }
    }
}