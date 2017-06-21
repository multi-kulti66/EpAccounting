// ///////////////////////////////////
// File: ClientEditStateTest.cs
// Last Change: 11.05.2017  20:23
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
    public class ClientEditStateTest
    {
        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            ClientEditState clientEditState = this.GetDefaultClientEditState();

            // Assert
            clientEditState.CanSwitchToSearchMode().Should().BeFalse();
            clientEditState.CanSwitchToAddMode().Should().BeFalse();
            clientEditState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommitAndSave()
        {
            // Arrange
            ClientEditState clientEditState = this.GetDefaultClientEditState();

            // Assert
            clientEditState.CanCommit().Should().BeTrue();
            clientEditState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            ClientEditState clientEditState = this.GetDefaultClientEditState();

            // Assert
            clientEditState.CanDelete().Should().BeFalse();
        }

        [Test]
        public async Task ChangeToClientLoadedStateWhenClientWasSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientEditState clientEditState = this.GetDefaultClientEditState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(null, It.IsAny<ClientLoadedState>()), Times.Once);
        }

        [Test]
        public async Task DoNotChangeToClientLoadedStateWhenClientWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientEditState clientEditState = this.GetDefaultClientEditState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(null, It.IsAny<ClientLoadedState>()), Times.Never);
        }

        [Test]
        public async Task SendUpdateClientValuesMessageWhenClientWasSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientEditState clientEditState = this.GetDefaultClientEditState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.SendUpdateClientValuesMessage(), Times.Once);
        }

        [Test]
        public async Task DoNotSendUpdateClientValuesMessageWhenClientWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientEditState clientEditState = this.GetDefaultClientEditState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.SendUpdateClientValuesMessage(), Times.Never);
        }

        [Test]
        public void ReloadClientValuesAndChangeToClientLoadedState()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetDefaultMockClientEditViewModel();
            ClientEditState clientEditState = this.GetDefaultClientEditState(mockClientEditViewModel);

            // Act
            clientEditState.Cancel();

            // Assert
            mockClientEditViewModel.Verify(x => x.Reload(), Times.Once);
        }

        private ClientEditState GetDefaultClientEditState()
        {
            return new ClientEditState(this.GetDefaultClientEditViewModel());
        }

        private ClientEditState GetDefaultClientEditState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientEditState(mockClientEditViewModel.Object);
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