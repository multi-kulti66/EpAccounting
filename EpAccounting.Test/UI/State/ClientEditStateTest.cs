// ///////////////////////////////////
// File: ClientEditStateTest.cs
// Last Change: 20.08.2017  16:13
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
            ClientEditState clientEditState = this.GetDefaultState();

            // Assert
            clientEditState.CanSwitchToSearchMode().Should().BeFalse();
            clientEditState.CanSwitchToAddMode().Should().BeFalse();
            clientEditState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommitAndSave()
        {
            // Arrange
            ClientEditState clientEditState = this.GetDefaultState();

            // Assert
            clientEditState.CanCommit().Should().BeTrue();
            clientEditState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            ClientEditState clientEditState = this.GetDefaultState();

            // Assert
            clientEditState.CanDelete().Should().BeFalse();
        }

        [Test]
        public async Task ChangeToClientLoadedStateWhenClientWasSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientEditState clientEditState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Once);
        }

        [Test]
        public async Task DoNotChangeToClientLoadedStateWhenClientWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientEditState clientEditState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Never);
        }

        [Test]
        public async Task SendUpdateClientValuesMessageWhenClientWasSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientEditState clientEditState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.SendUpdateClientValuesMessage(), Times.Once);
        }

        [Test]
        public async Task DoNotSendUpdateClientValuesMessageWhenClientWasNotSavedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientEditState clientEditState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientEditState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.SendUpdateClientValuesMessage(), Times.Never);
        }

        [Test]
        public void ReloadClientValuesAndChangeToClientLoadedState()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientEditState clientEditState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientEditState.Cancel();

            // Assert
            mockClientEditViewModel.Verify(x => x.Load(It.IsAny<int>()), Times.Once);
        }


        private ClientEditState GetDefaultState()
        {
            return new ClientEditState(this.GetMockedViewModel().Object);
        }

        private ClientEditState GetDefaultState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientEditState(mockClientEditViewModel.Object);
        }

        private Mock<ClientEditViewModel> GetMockedViewModel()
        {
            return new Mock<ClientEditViewModel>(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }
    }
}