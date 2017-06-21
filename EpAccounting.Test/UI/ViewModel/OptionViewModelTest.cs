// ///////////////////////////////////
// File: OptionViewModelTest.cs
// Last Change: 14.03.2017  16:32
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class OptionViewModelTest
    {
        private const string DefaultTitle = "Optionen";
        private readonly Bitmap DefaultImage = Resources.img_options;

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
            typeof(OptionViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void PropertiesInitializedAfterCreation()
        {
            // Act
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel();

            // Assert
            optionViewModel.Title.Should().Be(DefaultTitle);
            optionViewModel.Image.Should().Be(this.DefaultImage);
        }

        [Test]
        public void GetSavedDatabasePath()
        {
            // Arrange
            DatabaseFactory.SetSavedFilePath();

            // Act
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel();

            // Assert
            optionViewModel.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void DoNotCreateDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(() => null);
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void CreateDatabaseIfValidFolderPath()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns("Desktop\\EpAccounting");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void OverwriteExistingDatabaseIfDialogResultIsYes()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(Task.FromResult(true));

            DatabaseFactory.CreateTestFile();

            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotOverwriteExistingDatabaseIfDialogResultIsNo()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(Task.FromResult(false));

            DatabaseFactory.CreateTestFile();

            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            string ExpectedFolderPath = "Desktop\\EpAccounting\\";
            string ExpectedFilePath = ExpectedFolderPath + Resources.Database_NameWithExtension;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(ExpectedFolderPath);
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            optionViewModel.FilePath.Should().Be(ExpectedFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns("Desktop\\EpAccounting");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockDialogService);
            optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeCreated()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.CreateDatabase(It.IsAny<string>())).Throws(new Exception());
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns("Desktop\\EpAccounting");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(this);

            // Assert
            mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotCreateDatabase, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotLoadDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(() => null);
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void LoadDatabaseIfValidDatabaseFile()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns("Desktop\\EpAccounting\\EpAccounting.db");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            string ExpectedFilePath = "Desktop\\EpAccounting\\EpAccounting.db";

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(ExpectedFilePath);
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            Settings.Default.DatabaseFilePath.Should().Be(ExpectedFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns("Desktop\\EpAccounting\\EpAccounting.db");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockDialogService);
            optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeLoaded()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.LoadDatabase(It.IsAny<string>())).Throws(new Exception());
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns("Desktop\\EpAccounting\\EpAccounting.db");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(this);

            // Assert
            mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotLoadDatabase, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendUpdateConnectionStateMessageWhenDatabaseWasCreated()
        {
            // Arrange
            string message = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => message = x.Notification);

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowFolderDialog()).Returns("Desktop\\EpAccounting");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            message.Should().Be(Resources.Messenger_Message_UpdateConnectionState);
        }

        [Test]
        public void SendUpdateConnectionStateMessageWhenDatabaseWasLoaded()
        {
            // Arrange
            string message = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => message = x.Notification);

            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns("Desktop\\EpAccounting\\EpAccounting.db");
            OptionViewModel optionViewModel = this.GetDefaultOptionViewModel(mockRepository, mockDialogService);

            // Act
            optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            message.Should().Be(Resources.Messenger_Message_UpdateConnectionState);
        }

        private OptionViewModel GetDefaultOptionViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new OptionViewModel(DefaultTitle, this.DefaultImage, mockRepository.Object, mockDialogService.Object);
        }

        private OptionViewModel GetDefaultOptionViewModel(Mock<IDialogService> mockDialogService)
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();

            return new OptionViewModel(DefaultTitle, this.DefaultImage, mockRepository.Object, mockDialogService.Object);
        }

        private OptionViewModel GetDefaultOptionViewModel(Mock<IRepository> mockRepository, Mock<IDialogService> mockDialogService)
        {
            return new OptionViewModel(DefaultTitle, this.DefaultImage, mockRepository.Object, mockDialogService.Object);
        }
    }
}