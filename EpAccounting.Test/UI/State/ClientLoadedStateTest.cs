// ///////////////////////////////////
// File: ClientLoadedStateTest.cs
// Last Change: 20.08.2017  16:08
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
    public class ClientLoadedStateTest
    {
        #region Test Methods

        [Test]
        public void CanSwitchToAllModes()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultState();

            // Assert
            clientLoadedState.CanSwitchToSearchMode().Should().BeTrue();
            clientLoadedState.CanSwitchToAddMode().Should().BeTrue();
            clientLoadedState.CanSwitchToEditMode().Should().BeTrue();
        }

        [Test]
        public void CanNotCommitOrCancel()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultState();

            // Assert
            clientLoadedState.CanCommit().Should().BeFalse();
            clientLoadedState.CanCancel().Should().BeFalse();
        }

        [Test]
        public void CanDelete()
        {
            // Arrange
            ClientLoadedState clientLoadedState = this.GetDefaultState();

            // Assert
            clientLoadedState.CanDelete().Should().BeTrue();
        }

        [Test]
        public void LoadsNewClientAndSwitchesToSearchMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToSearchMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToSearchMode(), Times.Once);
        }

        [Test]
        public void LoadsNewClientAndSwitchesToAddMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToAddMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToCreationMode(), Times.Once);
        }

        [Test]
        public void SwitchesToEditMode()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientLoadedState clientLoadedState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientLoadedState.SwitchToEditMode();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToEditMode(), Times.Once);
        }

        [Test]
        public async Task LoadIntoClientEmptyStateWhenClientWasDeletedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.DeleteClientAsync()).Returns(Task.FromResult(true));

            ClientLoadedState clientLoadedState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientLoadedState.Delete();

            // Assert
            mockClientEditViewModel.Verify(x => x.DeleteClientAsync(), Times.Once);
            mockClientEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Once);
        }

        [Test]
        public async Task DoNothingWhenClientWasNotDeletedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.DeleteClientAsync()).Returns(Task.FromResult(false));

            ClientLoadedState clientLoadedState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientLoadedState.Delete();

            // Assert
            mockClientEditViewModel.Verify(x => x.DeleteClientAsync(), Times.Once);
            mockClientEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Never);
        }

        #endregion



        private ClientLoadedState GetDefaultState()
        {
            return new ClientLoadedState(this.GetMockedViewModel().Object);
        }

        private ClientLoadedState GetDefaultState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientLoadedState(mockClientEditViewModel.Object);
        }

        private Mock<ClientEditViewModel> GetMockedViewModel()
        {
            return new Mock<ClientEditViewModel>(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }
    }
}