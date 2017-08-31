// ///////////////////////////////////
// File: ClientSearchStateTest.cs
// Last Change: 11.05.2017  20:36
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
    public class ClientSearchStateTest
    {
        [Test]
        public void CanNotSwitchToOtherModes()
        {
            // Arrange
            ClientSearchState clientSearchState = this.GetDefaultState();

            // Assert
            clientSearchState.CanSwitchToSearchMode().Should().BeFalse();
            clientSearchState.CanSwitchToAddMode().Should().BeFalse();
            clientSearchState.CanSwitchToEditMode().Should().BeFalse();
        }

        [Test]
        public void CanCommit()
        {
            // Arrange
            ClientSearchState clientSearchState = this.GetDefaultState();

            // Assert
            clientSearchState.CanCommit().Should().BeTrue();
        }

        [Test]
        public void SendsClientSearchCriterionMessage()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientSearchState clientSearchState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientSearchState.Commit();

            // Assert
            mockClientEditViewModel.Verify(x => x.SendClientSearchCriterionMessage(), Times.Once);
        }

        [Test]
        public void CanCancel()
        {
            // Arrange
            ClientSearchState clientSearchState = this.GetDefaultState();

            // Assert
            clientSearchState.CanCancel().Should().BeTrue();
        }

        [Test]
        public void ReturnToEmptyClientState()
        {
            // Arrange
            Mock<ClientEditViewModel> mockClientEditViewModel = this.GetMockedViewModel();
            ClientSearchState clientSearchState = this.GetDefaultState(mockClientEditViewModel);

            // Act
            clientSearchState.Cancel();

            // Assert
            mockClientEditViewModel.Verify(x => x.ChangeToEmptyMode(), Times.Once);
        }

        [Test]
        public void CanNotDelete()
        {
            // Arrange
            ClientSearchState clientSearchState = this.GetDefaultState();

            // Assert
            clientSearchState.CanDelete().Should().BeFalse();
        }

        private ClientSearchState GetDefaultState()
        {
            return new ClientSearchState(this.GetMockedViewModel().Object);
        }

        private ClientSearchState GetDefaultState(Mock<ClientEditViewModel> mockClientEditViewModel)
        {
            return new ClientSearchState(mockClientEditViewModel.Object);
        }

        private Mock<ClientEditViewModel> GetMockedViewModel()
        {
            return new Mock<ClientEditViewModel>(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }
    }
}