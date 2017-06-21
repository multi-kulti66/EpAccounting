// ///////////////////////////////////
// File: ClientLoadedStateTest.cs
// Last Change: 11.05.2017  20:26
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
    public class ClientLoadedStateTest
    {
        [Test]
        public void CanSwitchToAllModes()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState();

            // Assert
            clientLoadedState.CanSwitchToSearchMode().Should().BeTrue();
            clientLoadedState.CanSwitchToAddMode().Should().BeTrue();
            clientLoadedState.CanSwitchToEditMode().Should().BeTrue();
        }

        [Test]
        public void CanNotCommitOrCancel()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState();

            // Assert
            clientLoadedState.CanCommit().Should().BeFalse();
            clientLoadedState.CanCancel().Should().BeFalse();
        }

        [Test]
        public void CanDelete()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState();

            // Assert
            clientLoadedState.CanDelete().Should().BeTrue();
        }

        [Test]
        public void LoadsNewClientAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToSearchMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientSearchState>()), Times.Once);
        }

        [Test]
        public void LoadsNewClientAndSwitchesToAddMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToAddMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientCreationState>()), Times.Once);
        }

        [Test]
        public void SwitchesToEditMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToEditMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(null, It.IsAny<ClientEditState>()), Times.Once);
        }

        [Test]
        public async Task LoadIntoClientEmptyStateWhenClientWasDeletedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.DeleteClientAsync()).Returns(Task.FromResult(true));

            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState(mockClientEditViewModel);

            // Act
            await clientLoadedState.Delete();

            // Assert
            mockClientEditViewModel.Verify(x => x.DeleteClientAsync(), Times.Once);
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientEmptyState>()), Times.Once);
        }

        [Test]
        public async Task DoNothingWhenClientWasNotDeletedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.DeleteClientAsync()).Returns(Task.FromResult(false));

            ClientLoadedState clientLoadedState = this.GetDefaultClientLoadedState(mockClientEditViewModel);

            // Act
            await clientLoadedState.Delete();

            // Assert
            mockClientEditViewModel.Verify(x => x.DeleteClientAsync(), Times.Once);
            mockClientEditViewModel.Verify(x => x.Load(new Client(), It.IsAny<ClientEmptyState>()), Times.Never);
        }

        private ClientLoadedState GetDefaultClientLoadedState()
        {
            return new ClientLoadedState(this.GetDefaultClientEditViewModel());
        }

        private ClientLoadedState GetDefaultClientLoadedState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientLoadedState(mockClientEditViewModel.Object);
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