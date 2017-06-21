// ///////////////////////////////////
// File: WorkspaceViewModelTest.cs
// Last Change: 13.03.2017  15:48
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.Drawing;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class WorkspaceViewModelTest
    {
        [Test]
        public void InheritsFromBindableViewModelBase()
        {
            typeof(WorkspaceViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanInitializeInstance()
        {
            // Arrange
            const string ExpectedTitle = "Test";
            Bitmap ExpectedImage = Resources.img_clients;

            // Act
            WorkspaceViewModel workspaceVM = new WorkspaceViewModel(ExpectedTitle, ExpectedImage);

            // Assert
            workspaceVM.Should().NotBeNull();
            workspaceVM.Title.Should().Be(ExpectedTitle);
            workspaceVM.Image.Should().Be(ExpectedImage);
        }
    }
}