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
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private ClientViewModel _clientViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._clientViewModel = new ClientViewModel(Resources.Workspace_Title_Clients, Resources.img_clients,
                                                       this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._clientViewModel = null;
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
            this._clientViewModel.ClientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetInitializedClientSearchViewModelAfterCreation()
        {
            // Assert
            this._clientViewModel.ClientSearchViewModel.Should().NotBeNull();
        }
    }
}