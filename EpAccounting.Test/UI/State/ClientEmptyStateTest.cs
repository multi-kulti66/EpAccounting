// ///////////////////////////////////
// File: ClientEmptyStateTest.cs
// Last Change: 20.08.2017  16:10
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
    public class ClientEmptyStateTest
    {
        #region Test Methods

        [Test]
        public void CanSwitchToSearchAndAddMode()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultState();

            // Assert
            clientEmptyState.CanSwitchToSearchMode().Should().BeTrue();
            clientEmptyState.CanSwitchToAddMode().Should().BeTrue();
        }

        [Test]
        public void CanNotSwitchToEditState()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultState();

            // Assert
            clientEmptyState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanNotCommitOrCancelOrDelete()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultState();

            // Assert
            clientEmptyState.CanCommit().Should().BeFalse();
            clientEmptyState.CanCancel().Should().BeFalse();
            clientEmptyState.CanDelete().Should().BeFalse();
        }

        [Test]
        public void LoadsNewClientAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientEmptyState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientCreationState.SwitchToSearchMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToSearchMode(), Times.Once);
        }

        [Test]
        public void LoadsNewClientAndSwitchesToAddMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientEmptyState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientCreationState.SwitchToAddMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToCreationMode(), Times.Once);
        }

        #endregion



        private ClientEmptyState GetDefaultState()
        {
            return new ClientEmptyState(this.GetMockedViewModel().Object);
        }

        private ClientEmptyState GetDefaultState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientEmptyState(mockClientEditViewModel.Object);
        }

        private Mock<ClientEditViewModel> GetMockedViewModel()
        {
            return new Mock<ClientEditViewModel>(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }
    }
}