// ///////////////////////////////////
// File: BillCreationStateTest.cs
// Last Change: 22.08.2017  20:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillCreationStateTest
    {
        #region Test Methods

        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            BillCreationState billCreationState = this.GetDefaultBillCreationState();

            // Assert
            billCreationState.CanSwitchToSearchMode().Should().BeFalse();
            billCreationState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommit()
        {
            // Arrange
            BillCreationState billCreationState = this.GetDefaultBillCreationState();

            // Assert
            billCreationState.CanCommit().Should().BeTrue();
        }

        [Test]
        public async Task ChangeToBillLoadedStateWhenBillWasAddedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(true));

            BillCreationState billCreationState = this.GetDefaultBillCreationState(mockBillEditViewModel);

            // Act
            await billCreationState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Once);
        }

        [Test]
        public async Task DoNotChangeToBillLoadedStateWhenBillWasNotAddedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(false));

            BillCreationState billCreationState = this.GetDefaultBillCreationState(mockBillEditViewModel);

            // Act
            await billCreationState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Never);
        }

        [Test]
        public void CanCancel()
        {
            // Arrange
            BillCreationState billCreationState = this.GetDefaultBillCreationState();

            // Assert
            billCreationState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void ReturnToPreviousBillAndBillState()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            BillCreationState billCreationState = this.GetDefaultBillCreationState(mockBillEditViewModel);

            // Act
            billCreationState.Cancel();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Once);
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            BillCreationState billCreationState = this.GetDefaultBillCreationState();

            // Assert
            billCreationState.CanDelete().Should().BeFalse();
        }

        #endregion



        private BillCreationState GetDefaultBillCreationState()
        {
            return new BillCreationState(this.GetDefaultBillEditViewModel());
        }

        private BillCreationState GetDefaultBillCreationState(Mock<BillEditViewModel> mockBillEditViewModel)
        {
            return new BillCreationState(mockBillEditViewModel.Object);
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