﻿// ///////////////////////////////////
// File: ClientCreationStateTest.cs
// Last Change: 20.08.2017  16:15
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;
    using Resources = EpAccounting.UI.Properties.Resources;


    [TestFixture]
    public class ClientCreationStateTest
    {
        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultState();

            // Assert
            clientCreationState.CanSwitchToSearchMode().Should().BeFalse();
            clientCreationState.CanSwitchToAddMode().Should().BeFalse();
            clientCreationState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommit()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultState();

            // Assert
            clientCreationState.CanCommit().Should().BeTrue();
        }

        [Test]
        public async Task ChangeToClientLoadedStateWhenClientWasAddedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            ClientCreationState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientCreationState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Once);
        }

        [Test]
        public async Task SendReloadClientSearchMessageWhenClientWasAddedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(true));

            NotificationMessage notificationMessage = null;
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessage = x);

            ClientCreationState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientCreationState.Commit();

            // Assert
            notificationMessage.Should().NotBeNull();
            notificationMessage.Notification.Should().Be(Resources.Message_ReloadClientsForClientSearchVM);
        }

        [Test]
        public async Task DoNotChangeToClientLoadedStateWhenClientWasNotAddedSuccessfully()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            mockClientEditViewModel.Setup(x => x.SaveOrUpdateClientAsync()).Returns(Task.FromResult(false));

            ClientCreationState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            await clientCreationState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToLoadedMode(null), Times.Never);
        }

        [Test]
        public void CanCancel()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultState();

            // Assert
            clientCreationState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void ReturnToPreviousClientAndClientState()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientCreationState clientCreationState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientCreationState.Cancel();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Once);
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            ClientCreationState clientCreationState = this.GetDefaultState();

            // Assert
            clientCreationState.CanDelete().Should().BeFalse();
        }


        private ClientCreationState GetDefaultState()
        {
            return new ClientCreationState(this.GetMockedViewModel().Object);
        }

        private ClientCreationState GetDefaultState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientCreationState(mockClientEditViewModel.Object);
        }

        private Mock<ClientEditViewModel> GetMockedViewModel()
        {
            return new Mock<ClientEditViewModel>(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }
    }
}