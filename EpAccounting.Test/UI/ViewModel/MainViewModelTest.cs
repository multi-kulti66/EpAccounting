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
        #region Fields

        private MainViewModel mainViewModel;

        #endregion



        #region Setup/Teardown

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

            this.mainViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

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
            this.mainViewModel = this.GetMockedViewModel();

            // Assert
            this.mainViewModel.WorkspaceViewModels.Should().HaveCount(4);
            this.mainViewModel.CurrentWorkspace.Should().NotBeNull();
            this.mainViewModel.CurrentWorkspace.Should().BeOfType<ClientViewModel>();
        }

        [Test]
        public void ConnectAtStartupWhenSavedDatabasePathIsValid()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            DatabaseFactory.SetSavedFilePath();

            // Act
            this.mainViewModel = this.GetDefaultViewModel();

            // Assert
            this.mainViewModel.IsConnected.Should().BeTrue();
        }

        [Test]
        public void DoNotConnectAtStartupWhenSavedDatabasePathIsInvalid()
        {
            // Arrange
            this.mainViewModel = this.GetDefaultViewModel();

            // Assert
            this.mainViewModel.IsConnected.Should().BeFalse();
        }

        [Test]
        public void ClearSavedDatabasePathWhenSavedPathWasInvalidAtStartup()
        {
            // Arrange
            Settings.Default.DatabaseFilePath = "Desktop\\Test.db";

            // Act
            this.mainViewModel = this.GetDefaultViewModel();

            // Assert
            Settings.Default.DatabaseFilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void UpdateConnectionState()
        {
            // Arrange
            this.mainViewModel = this.GetMockedViewModel();
            this.mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateConnectionStateForMainVM));

            // Assert
            this.mainViewModel.ShouldRaisePropertyChangeFor(x => x.IsConnected);
        }

        [Test]
        public void ChangeToBillWorkspace()
        {
            // Arrange
            this.mainViewModel = this.GetMockedViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));

            // Assert
            this.mainViewModel.CurrentWorkspace.Should().BeOfType<BillViewModel>();
        }

        [Test]
        public void RaisePropertyChangedWhenCurrentViewModelChanges()
        {
            // Arrange
            this.mainViewModel = this.GetMockedViewModel();
            this.mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));

            // Assert
            this.mainViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentWorkspace);
        }

        [Test]
        public void CanChangeWorkspaceAfterInitialization()
        {
            // Act
            this.mainViewModel = this.GetMockedViewModel();

            // Assert
            this.mainViewModel.CanChangeWorkspace.Should().BeTrue();
        }

        [Test]
        public void DisableWorkspaceChangingWithMessage()
        {
            // Arrange
            this.mainViewModel = this.GetMockedViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_WorkspaceEnableStateForMainVM));

            // Assert
            this.mainViewModel.CanChangeWorkspace.Should().BeFalse();
        }

        #endregion



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