// ///////////////////////////////////
// File: ClientViewModelTest.cs
// Last Change: 18.03.2017  21:33
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using EpAccounting.Business;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class ClientViewModelTest
    {
        [Test]
        public void DerivesFromWorkspaceViewModel()
        {
            // Assert
            typeof(ClientViewModel).Should().BeDerivedFrom<WorkspaceViewModel>();
        }

        [Test]
        public void GetInitializedClientEditViewModelAfterCreation()
        {
            // Act
            ClientViewModel clientViewModel = this.GetClientViewModel();

            // Assert
            clientViewModel.ClientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetInitializedClientSearchViewModelAfterCreation()
        {
            // Act
            ClientViewModel clientViewModel = this.GetClientViewModel();

            // Assert
            clientViewModel.ClientSearchViewModel.Should().NotBeNull();
        }

        private ClientViewModel GetClientViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new ClientViewModel("Kunde", Resources.img_clients, mockRepository.Object, mockDialogService.Object);
        }
    }
}