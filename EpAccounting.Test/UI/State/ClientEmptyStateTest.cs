// ///////////////////////////////////
// File: ClientEmptyStateTest.cs
// Last Change: 16.03.2017  21:15
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class ClientEmptyStateTest
    {
        [Test]
        public void CanSwitchToSearchAndAddMode()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultClientEmptyState();

            // Assert
            clientEmptyState.CanSwitchToSearchMode().Should().BeTrue();
            clientEmptyState.CanSwitchToAddMode().Should().BeTrue();
        }

        [Test]
        public void CanNotSwitchToEditState()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultClientEmptyState();

            // Assert
            clientEmptyState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanNotCommitOrCancelOrDelete()
        {
            // Arrange
            ClientEmptyState clientEmptyState = this.GetDefaultClientEmptyState();

            // Assert
            clientEmptyState.CanCommit().Should().BeFalse();
            clientEmptyState.CanCancel().Should().BeFalse();
            clientEmptyState.CanDelete().Should().BeFalse();
        }

        [Test]
        public void LoadsNewClientAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientEmptyState clientCreationState = this.GetDefaultClientEmptyState(mockClientEditViewModel);

            // Act
            clientCreationState.SwitchToSearchMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientSearchState>()), Times.Once);
        }

        [Test]
        public void LoadsNewClientAndSwitchesToAddMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientEmptyState clientCreationState = this.GetDefaultClientEmptyState(mockClientEditViewModel);

            // Act
            clientCreationState.SwitchToAddMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientCreationState>()), Times.Once);
        }

        private ClientEmptyState GetDefaultClientEmptyState()
        {
            return new ClientEmptyState(this.GetDefaultClientEditViewModel());
        }

        private ClientEmptyState GetDefaultClientEmptyState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientEmptyState(mockClientEditViewModel.Object);
        }

        private ClientEditViewModel GetDefaultClientEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new ClientEditViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private Mock<ClientEditViewModel> GetDefaultMockClientEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            return new Mock<ClientEditViewModel>(mockRepository.Object, mockDialogService.Object);
        }
    }
}