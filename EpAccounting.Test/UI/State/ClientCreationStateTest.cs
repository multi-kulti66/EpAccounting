// ///////////////////////////////////
// File: ClientCreationStateTest.cs
// Last Change: 11.05.2017  20:14
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
    public class ClientCreationStateTest
    {
        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultClientCreationState();

            // Assert
            clientCreationState.CanSwitchToSearchMode().Should().BeFalse();
            clientCreationState.CanSwitchToAddMode().Should().BeFalse();
            clientCreationState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommit()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultClientCreationState();

            // Assert
            clientCreationState.CanCommit().Should().BeTrue();
        }

        [Test]
        public async Task ChangeToClientLoadedStateWhenClientWasAddedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientCreationState clientCreationState = this.GetDefaultClientCreationState(mockClientEditViewModel);

            // Act
            await clientCreationState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(null, It.IsAny<ClientLoadedState>()), Times.Once);
        }

        [Test]
        public async Task DoNotChangeToClientLoadedStateWhenClientWasNotAddedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientCreationState clientCreationState = this.GetDefaultClientCreationState(mockClientEditViewModel);

            // Act
            await clientCreationState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(null, It.IsAny<ClientLoadedState>()), Times.Never);
        }

        [Test]
        public void CanCancel()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultClientCreationState();

            // Assert
            clientCreationState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void ReturnToPreviousClientAndClientState()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientCreationState clientCreationState = this.GetDefaultClientCreationState(mockClientEditViewModel);

            // Act
            clientCreationState.Cancel();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientEmptyState>()), Times.Once);
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultClientCreationState();

            // Assert
            clientCreationState.CanDelete().Should().BeFalse();
        }

        private ClientCreationState GetDefaultClientCreationState()
        {
            return new ClientCreationState(this.GetDefaultClientEditViewModel());
        }

        private ClientCreationState GetDefaultClientCreationState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientCreationState(mockClientEditViewModel.Object);
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