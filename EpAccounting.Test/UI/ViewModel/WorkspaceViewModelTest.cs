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
        private WorkspaceViewModel _workspaceViewModel;


        [SetUp]
        public void Init()
        {
            this._workspaceViewModel = new WorkspaceViewModel(Resources.Workspace_Title_Options, Resources.img_options);
        }

        [TearDown]
        public void Cleanup()
        {
            this._workspaceViewModel = null;
            GC.Collect();
        }


        [Test]
        public void InheritsFromBindableViewModelBase()
        {
            typeof(WorkspaceViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanInitializeInstance()
        {
            // Assert
            this._workspaceViewModel.Should().NotBeNull();
            this._workspaceViewModel.Title.Should().Be(Resources.Workspace_Title_Options);
            this._workspaceViewModel.Image.FrameDimensionsList.ShouldBeEquivalentTo(Resources.img_options.FrameDimensionsList);
        }
    }
}