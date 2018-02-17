// ///////////////////////////////////
// File: ClientViewModelTest.cs
// Last Change: 23.08.2017  20:36
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
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
        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private ClientViewModel clientViewModel;


        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.clientViewModel = new ClientViewModel(Resources.Workspace_Title_Clients, Resources.img_clients,
                                                       this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.clientViewModel = null;
            GC.Collect();
        }


        [Test]
        public void DerivesFromWorkspaceViewModel()
        {
            // Assert
            typeof(ClientViewModel).Should().BeDerivedFrom<WorkspaceViewModel>();
        }

        [Test]
        public void GetInitializedClientEditViewModelAfterCreation()
        {
            // Assert
            this.clientViewModel.ClientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetInitializedClientSearchViewModelAfterCreation()
        {
            // Assert
            this.clientViewModel.ClientSearchViewModel.Should().NotBeNull();
        }
    }
}