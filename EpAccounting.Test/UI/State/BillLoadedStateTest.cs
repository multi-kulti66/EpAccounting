// ///////////////////////////////////
// File: BillLoadedStateTest.cs
// Last Change: 18.06.2017  19:07
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillLoadedStateTest
    {
        #region Test Methods

        [Test]
        public void CanSwitchToAllModes()
        {
            // Arrange
            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState();

            // Assert
            billLoadedState.CanSwitchToSearchMode().Should().BeTrue();
            billLoadedState.CanSwitchToEditMode().Should().BeTrue();
        }

        [Test]
        public void CanNotCommitOrCancel()
        {
            // Arrange
            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState();

            // Assert
            billLoadedState.CanCommit().Should().BeFalse();
            billLoadedState.CanCancel().Should().BeFalse();
        }

        [Test]
        public void CanDelete()
        {
            // Arrange
            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState();

            // Assert
            billLoadedState.CanDelete().Should().BeTrue();
        }

        [Test]
        public void LoadsNewBillAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState(mockBillEditViewModel);

            // Act
            billLoadedState.SwitchToSearchMode();

            // Assert
            mockBillEditViewModel.Verify(x => x.Load(new Bill() { Client = new Client() }, It.IsAny<BillSearchState>()), Times.Once);
        }

        [Test]
        public void SwitchesToEditMode()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState(mockBillEditViewModel);

            // Act
            billLoadedState.SwitchToEditMode();

            // Assert
            mockBillEditViewModel.Verify(x => x.Load(null, It.IsAny<BillEditState>()), Times.Once);
        }

        [Test]
        public async Task LoadIntoBillEmptyStateWhenBillWasDeletedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.DeleteBillAsync()).Returns(Task.FromResult(true));

            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState(mockBillEditViewModel);

            // Act
            await billLoadedState.Delete();

            // Assert
            mockBillEditViewModel.Verify(x => x.DeleteBillAsync(), Times.Once);
            mockBillEditViewModel.Verify(x => x.Load(new Bill() {Client = new Client()}, It.IsAny<BillEmptyState>()), Times.Once);
        }

        [Test]
        public async Task DoNothingWhenBillWasNotDeletedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.DeleteBillAsync()).Returns(Task.FromResult(false));

            BillLoadedState billLoadedState = this.GetDefaultBillLoadedState(mockBillEditViewModel);

            // Act
            await billLoadedState.Delete();

            // Assert
            mockBillEditViewModel.Verify(x => x.DeleteBillAsync(), Times.Once);
            mockBillEditViewModel.Verify(x => x.Load(new Bill() {Client = new Client()}, It.IsAny<BillEmptyState>()), Times.Never);
        }

        #endregion



        private BillLoadedState GetDefaultBillLoadedState()
        {
            return new BillLoadedState(this.GetDefaultBillEditViewModel());
        }

        private BillLoadedState GetDefaultBillLoadedState(Mock<BillEditViewModel> mockBillEditViewModel)
        {
            return new BillLoadedState(mockBillEditViewModel.Object);
        }

        private BillEditViewModel GetDefaultBillEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new BillEditViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private Mock<BillEditViewModel> GetDefaultMockBillEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            return new Mock<BillEditViewModel>(mockRepository.Object, mockDialogService.Object);
        }
    }
}