// ///////////////////////////////////
// File: OptionViewModelTest.cs
// Last Change: 23.08.2017  20:43
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
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
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private OptionViewModel _optionViewModel;


        [SetUp]
        public void Init()
        {
            DatabaseFactory.ClearSavedFilePath();

            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._optionViewModel = new OptionViewModel(Resources.Workspace_Title_Options, Resources.img_options, this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void CleanUp()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.ClearSavedFilePath();

            this._mockRepository = null;
            this._mockDialogService = null;
            this._optionViewModel = null;
            GC.Collect();
        }


        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(OptionViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void PropertiesInitializedAfterCreation()
        {
            // Assert
            this._optionViewModel.Title.Should().Be(Resources.Workspace_Title_Options);
            this._optionViewModel.Image.FrameDimensionsList.Should().BeEquivalentTo(Resources.img_options.FrameDimensionsList);
        }

        [Test]
        public void GetSavedDatabasePath()
        {
            // Act
            DatabaseFactory.SetSavedFilePath();

            // Assert
            this._optionViewModel.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void DoNotCreateDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(() => null);

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void CreateDatabaseIfValidFolderPath()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._optionViewModel.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this._optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void UpdatesConnectionStateAfterDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            string receivedMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => receivedMessage = x.Notification);

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            receivedMessage.Should().Be(Resources.Message_UpdateConnectionStateForMainVM);
        }

        [Test]
        public void OverwriteExistingDatabaseIfDialogResultIsYes()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            DatabaseFactory.CreateTestFile();

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotOverwriteExistingDatabaseIfDialogResultIsNo()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            DatabaseFactory.CreateTestFile();

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeCreated()
        {
            // Arrange
            this._mockRepository.Setup(x => x.CreateDatabase(It.IsAny<string>())).Throws(new Exception());
            this._mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this._optionViewModel.CreateDatabaseCommand.Execute(this);

            // Assert
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotCreateDatabase, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotLoadDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(() => null);

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void LoadDatabaseIfValidDatabaseFile()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this._mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            Settings.Default.DatabaseFilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);
            this._optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this._optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void UpdatesConnectionStateAfterDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            string receivedMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => receivedMessage = x.Notification);

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            receivedMessage.Should().Be(Resources.Message_UpdateConnectionStateForMainVM);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeLoaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.LoadDatabase(It.IsAny<string>())).Throws(new Exception());
            this._mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this._optionViewModel.LoadDatabaseCommand.Execute(this);

            // Assert
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotLoadDatabase, It.IsAny<string>()), Times.Once);
        }
    }
}