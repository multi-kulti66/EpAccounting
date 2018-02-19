// ///////////////////////////////////
// File: MainViewModelTest.cs
// Last Change: 27.10.2017  21:53
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using EpAccounting.Business;
    using EpAccounting.Data;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class MainViewModelTest
    {
        private MainViewModel _mainViewModel;


        [SetUp]
        public void Init()
        {
            DatabaseFactory.ClearSavedFilePath();
        }

        [TearDown]
        public void CleanUp()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.ClearSavedFilePath();

            this._mainViewModel = null;
            GC.Collect();
        }


        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            // Assert
            typeof(MainViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void WorkspacesInitializedAfterCreation()
        {
            // Act
            this._mainViewModel = this.GetMockedViewModel();

            // Assert
            this._mainViewModel.WorkspaceViewModels.Should().HaveCount(4);
            this._mainViewModel.CurrentWorkspace.Should().NotBeNull();
            this._mainViewModel.CurrentWorkspace.Should().BeOfType<ClientViewModel>();
        }

        [Test]
        public void ConnectAtStartupWhenSavedDatabasePathIsValid()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            DatabaseFactory.SetSavedFilePath();

            // Act
            this._mainViewModel = this.GetDefaultViewModel();

            // Assert
            this._mainViewModel.IsConnected.Should().BeTrue();
        }

        [Test]
        public void DoNotConnectAtStartupWhenSavedDatabasePathIsInvalid()
        {
            // Arrange
            this._mainViewModel = this.GetDefaultViewModel();

            // Assert
            this._mainViewModel.IsConnected.Should().BeFalse();
        }

        [Test]
        public void ClearSavedDatabasePathWhenSavedPathWasInvalidAtStartup()
        {
            // Arrange
            Settings.Default.DatabaseFilePath = "Desktop\\Test.db";

            // Act
            this._mainViewModel = this.GetDefaultViewModel();

            // Assert
            Settings.Default.DatabaseFilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void UpdateConnectionState()
        {
            // Arrange
            this._mainViewModel = this.GetMockedViewModel();
            this._mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateConnectionStateForMainVM));

            // Assert
            this._mainViewModel.ShouldRaisePropertyChangeFor(x => x.IsConnected);
        }

        [Test]
        public void ChangeToBillWorkspace()
        {
            // Arrange
            this._mainViewModel = this.GetMockedViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));

            // Assert
            this._mainViewModel.CurrentWorkspace.Should().BeOfType<BillViewModel>();
        }

        [Test]
        public void RaisePropertyChangedWhenCurrentViewModelChanges()
        {
            // Arrange
            this._mainViewModel = this.GetMockedViewModel();
            this._mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));

            // Assert
            this._mainViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentWorkspace);
        }

        [Test]
        public void CanChangeWorkspaceAfterInitialization()
        {
            // Act
            this._mainViewModel = this.GetMockedViewModel();

            // Assert
            this._mainViewModel.CanChangeWorkspace.Should().BeTrue();
        }

        [Test]
        public void DisableWorkspaceChangingWithMessage()
        {
            // Arrange
            this._mainViewModel = this.GetMockedViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_WorkspaceEnableStateForMainVM));

            // Assert
            this._mainViewModel.CanChangeWorkspace.Should().BeFalse();
        }


        private MainViewModel GetDefaultViewModel()
        {
            ISessionManager sessionManager = new NHibernateSessionManager();
            IRepository repository = new NHibernateRepository(sessionManager);
            IDialogService dialogService = new DialogService();

            return new MainViewModel(repository, dialogService);
        }

        private MainViewModel GetMockedViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new MainViewModel(mockRepository.Object, mockDialogService.Object);
        }
    }
}