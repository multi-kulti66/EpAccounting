// ///////////////////////////////////
// File: WorkspaceViewModelTest.cs
// Last Change: 23.08.2017  20:49
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class WorkspaceViewModelTest
    {
        #region Fields

        private WorkspaceViewModel workspaceViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.workspaceViewModel = new WorkspaceViewModel(Resources.Workspace_Title_Options, Resources.img_options);
        }

        [TearDown]
        public void Cleanup()
        {
            this.workspaceViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void InheritsFromBindableViewModelBase()
        {
            typeof(WorkspaceViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanInitializeInstance()
        {
            // Assert
            this.workspaceViewModel.Should().NotBeNull();
            this.workspaceViewModel.Title.Should().Be(Resources.Workspace_Title_Options);
            this.workspaceViewModel.Image.FrameDimensionsList.ShouldBeEquivalentTo(Resources.img_options.FrameDimensionsList);
        }

        #endregion
    }
}