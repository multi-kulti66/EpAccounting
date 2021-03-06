﻿// ///////////////////////////////////
// File: BillEmptyStateTest.cs
// Last Change: 22.08.2017  20:45
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



    public class BillEmptyStateTest
    {
        [Test]
        public void CanSwitchToSearchAndAddMode()
        {
            // Arrange
            BillEmptyState billEmptyState = this.GetDefaultBillEmptyState();

            // Assert
            billEmptyState.CanSwitchToSearchMode().Should().BeTrue();
        }

        [Test]
        public void CanNotSwitchToEditState()
        {
            // Arrange
            BillEmptyState billEmptyState = this.GetDefaultBillEmptyState();

            // Assert
            billEmptyState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanNotCommitOrCancelOrDelete()
        {
            // Arrange
            BillEmptyState billEmptyState = this.GetDefaultBillEmptyState();

            // Assert
            billEmptyState.CanCommit().Should().BeFalse();
            billEmptyState.CanCancel().Should().BeFalse();
            billEmptyState.CanDelete().Should().BeFalse();
        }

        [Test]
        public void LoadsNewBillAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<BillEditViewModel> mockBillEditViewModel = this.GetDefaultMockBillEditViewModel();
            BillEmptyState billCreationState = this.GetDefaultBillEmptyState(mockBillEditViewModel);

            // Act
            billCreationState.SwitchToSearchMode();

            // Assert
            mockBillEditViewModel.Verify(x => x.ChangeToSearchMode(), Times.Once);
        }


        private BillEmptyState GetDefaultBillEmptyState()
        {
            return new BillEmptyState(this.GetDefaultBillEditViewModel());
        }

        private BillEmptyState GetDefaultBillEmptyState(Mock<BillEditViewModel> mockBillEditViewModel)
        {
            return new BillEmptyState(mockBillEditViewModel.Object);
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