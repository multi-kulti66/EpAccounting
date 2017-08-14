// ///////////////////////////////////
// File: MainViewModelTest.cs
// Last Change: 14.03.2017  15:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
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
            MainViewModel mainViewModel = this.GetMainViewModel();

            // Assert
            mainViewModel.WorkspaceViewModels.Should().HaveCount(2);
            mainViewModel.WorkspaceViewModels.Should().Contain(x => x.GetType() == typeof(ClientViewModel));
            mainViewModel.WorkspaceViewModels.Should().Contain(x => x.GetType() == typeof(OptionViewModel));
        }

        [Test]
        public void CurrentWorkspaceSetToFirstMenuWorkspace()
        {
            // Arrange
            MainViewModel mainViewModel = this.GetMainViewModel();

            // Assert
            mainViewModel.CurrentWorkspaceViewModel.Should().BeSameAs(mainViewModel.WorkspaceViewModels[0]);
        }

        [Test]
        public void RaisePropertyChangedWhenCurrentViewModelChanges()
        {
            // Arrange
            MainViewModel mainViewModel = this.GetMainViewModel();
            mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            mainViewModel.CurrentWorkspaceViewModel = mainViewModel.WorkspaceViewModels[1];

            // Assert
            mainViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentWorkspaceViewModel);
        }

        [Test]
        public void ConnectAtStartupWhenSavedDatabasePathIsValid()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            DatabaseFactory.SetSavedFilePath();

            // Act
            MainViewModel mainViewModel = new MainViewModel(new NHibernateRepository(new NHibernateSessionManager()), new DialogService());

            // Assert
            mainViewModel.IsConnected.Should().BeTrue();
        }

        [Test]
        public void DoNotConnectAtStartupWhenSavedDatabasePathIsInvalid()
        {
            // Act
            MainViewModel mainViewModel = new MainViewModel(new NHibernateRepository(new NHibernateSessionManager()), new DialogService());

            // Assert
            mainViewModel.IsConnected.Should().BeFalse();
        }

        [Test]
        public void ClearSavedDatabasePathWhenSavedPathWasInvalidAtStartup()
        {
            // Arrange
            Settings.Default.DatabaseFilePath = "Desktop\\Test.db";

            // Act
            MainViewModel mainViewModel = new MainViewModel(new NHibernateRepository(new NHibernateSessionManager()), new DialogService());


            // Assert
            Settings.Default.DatabaseFilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void UpdateConnectionStateWhenConnectionChangedMessageReceived()
        {
            // Arrange
            MainViewModel mainViewModel = this.GetMainViewModel();
            mainViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_UpdateConnectionStateMessageForMainVM));

            // Assert
            mainViewModel.ShouldRaisePropertyChangeFor(x => x.IsConnected);
        }

        [Test]
        public void ChangeToBillWorkspaceWhenCreateNewBillMessageReceived()
        {
            // Arrange
            MainViewModel mainViewModel = this.GetMainViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_CreateNewBillMessageForBillEditVM));

            // Assert
            mainViewModel.CurrentWorkspaceViewModel.Should().BeOfType<BillViewModel>();
        }

        private MainViewModel GetMainViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new MainViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private MainViewModel GetMainViewModel(Mock<IRepository> mockRepository)
        {
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new MainViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private MainViewModel GetMainViewModel(Mock<IDialogService> mockDialogService)
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();

            return new MainViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private MainViewModel GetMainViewModel(Mock<IRepository> mockRepository, Mock<IDialogService> mockDialogService)
        {
            return new MainViewModel(mockRepository.Object, mockDialogService.Object);
        }
    }
}