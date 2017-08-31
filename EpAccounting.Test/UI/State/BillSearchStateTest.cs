// ///////////////////////////////////
// File: BillSearchStateTest.cs
// Last Change: 22.08.2017  20:46
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using EpAccounting.Business;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillSearchStateTest
    {
        #region Test Methods

        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            BillSearchState billSearchState = this.GetDefaultState();

            // Assert
            billSearchState.CanSwitchToSearchMode().Should().BeFalse();
            billSearchState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommit()
        {
            // Arrange
            BillSearchState billSearchState = this.GetDefaultState();

            // Assert
            billSearchState.CanCommit().Should().BeTrue();
        }

        [Test]
        public void SendsBillSearchCriterionMessage()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockEditViewModel();
            BillSearchState billSearchState = this.GetDefaultState(mockBillEditViewModel);

            // Act
            billSearchState.Commit();

            // Assert
            mockBillEditViewModel.Verify(x => x.SendBillSearchCriterionMessage(), Times.Once);
        }

        [Test]
        public void CanCancel()
        {
            // Arrange
            BillSearchState billSearchState = this.GetDefaultState();

            // Assert
            billSearchState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void ReturnToEmptyBillState()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockEditViewModel();
            BillSearchState billSearchState = this.GetDefaultState(mockBillEditViewModel);

            // Act
            billSearchState.Cancel();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Once);
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            BillSearchState billSearchState = this.GetDefaultState();

            // Assert
            billSearchState.CanDelete().Should().BeFalse();
        }

        #endregion



        private BillSearchState GetDefaultState()
        {
            return new BillSearchState(this.GetDefaultMockEditViewModel().Object);
        }

        private BillSearchState GetDefaultState(Mock<BillEditViewModel> mockBillEditViewModel)
        {
            return new BillSearchState(mockBillEditViewModel.Object);
        }

        private Mock<BillEditViewModel> GetDefaultMockEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            return new Mock<BillEditViewModel>(mockRepository.Object, mockDialogService.Object);
        }
    }
}