// ///////////////////////////////////
// File: BillEditStateTest.cs
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



    public class BillEditStateTest
    {
        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            BillEditState billEditState = this.GetDefaultBillEditState();

            // Assert
            billEditState.CanSwitchToSearchMode().Should().BeFalse();
            billEditState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommitAndSave()
        {
            // Arrange
            BillEditState billEditState = this.GetDefaultBillEditState();

            // Assert
            billEditState.CanCommit().Should().BeTrue();
            billEditState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            BillEditState billEditState = this.GetDefaultBillEditState();

            // Assert
            billEditState.CanDelete().Should().BeFalse();
        }

        [Test]
        public async Task ChangeToBillLoadedStateWhenBillWasSavedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(true));

            BillEditState billEditState = this.GetDefaultBillEditState(mockBillEditViewModel);

            // Act
            await billEditState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Once);
        }

        [Test]
        public async Task DoNotChangeToBillLoadedStateWhenBillWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(false));

            BillEditState billEditState = this.GetDefaultBillEditState(mockBillEditViewModel);

            // Act
            await billEditState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Never);
        }

        [Test]
        public async Task SendUpdateBillValuesMessageWhenBillWasSavedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(true));

            BillEditState billEditState = this.GetDefaultBillEditState(mockBillEditViewModel);

            // Act
            await billEditState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.SendUpdateBillValuesMessage(), Times.Once);
        }

        [Test]
        public async Task DoNotSendUpdateBillValuesMessageWhenBillWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            mockBillEditViewModel.Setup(x => x.SaveOrUpdateBillAsync()).Returns(Task.FromResult(false));

            BillEditState billEditState = this.GetDefaultBillEditState(mockBillEditViewModel);

            // Act
            await billEditState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.SendUpdateBillValuesMessage(), Times.Never);
        }

        [Test]
        public void ReloadBillValuesAndChangeToBillLoadedState()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            BillEditState billEditState = this.GetDefaultBillEditState(mockBillEditViewModel);

            // Act
            billEditState.Cancel();

            // Assert
            mockBillEditViewModel.Verify(x => x.Reload(It.IsAny<int>()), Times.Once);
        }


        private BillEditState GetDefaultBillEditState()
        {
            return new BillEditState(this.GetDefaultBillEditViewModel());
        }

        private BillEditState GetDefaultBillEditState(Mock<BillEditViewModel> mockBillEditViewModel)
        {
            return new BillEditState(mockBillEditViewModel.Object);
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