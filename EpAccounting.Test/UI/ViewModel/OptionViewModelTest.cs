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
        #region Fields

        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private OptionViewModel optionViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            DatabaseFactory.ClearSavedFilePath();

            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.optionViewModel = new OptionViewModel(Resources.Workspace_Title_Options, Resources.img_options, this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void CleanUp()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.ClearSavedFilePath();

            this.mockRepository = null;
            this.mockDialogService = null;
            this.optionViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(OptionViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void PropertiesInitializedAfterCreation()
        {
            // Assert
            this.optionViewModel.Title.Should().Be(Resources.Workspace_Title_Options);
            this.optionViewModel.Image.FrameDimensionsList.Should().BeEquivalentTo(Resources.img_options.FrameDimensionsList);
        }

        [Test]
        public void GetSavedDatabasePath()
        {
            // Act
            DatabaseFactory.SetSavedFilePath();

            // Assert
            this.optionViewModel.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void DoNotCreateDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(() => null);

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void CreateDatabaseIfValidFolderPath()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.optionViewModel.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this.optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void UpdatesConnectionStateAfterDatabaseWasCreatedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            string receivedMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => receivedMessage = x.Notification);

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            receivedMessage.Should().Be(Resources.Message_UpdateConnectionStateForMainVM);
        }

        [Test]
        public void OverwriteExistingDatabaseIfDialogResultIsYes()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            DatabaseFactory.CreateTestFile();

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotOverwriteExistingDatabaseIfDialogResultIsNo()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            DatabaseFactory.CreateTestFile();

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.CreateDatabase(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeCreated()
        {
            // Arrange
            this.mockRepository.Setup(x => x.CreateDatabase(It.IsAny<string>())).Throws(new Exception());
            this.mockDialogService.Setup(x => x.ShowFolderDialog()).Returns(DatabaseFactory.TestFolderPath);

            // Act
            this.optionViewModel.CreateDatabaseCommand.Execute(this);

            // Assert
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotCreateDatabase, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotLoadDatabaseWhenNoFolderWasSelected()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(() => null);

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void LoadDatabaseIfValidDatabaseFile()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this.mockRepository.Verify(x => x.LoadDatabase(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveDatabaseFilePathInSettingsWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            Settings.Default.DatabaseFilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void RaisePropertyChangedWhenDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);
            this.optionViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            this.optionViewModel.ShouldRaisePropertyChangeFor(x => x.FilePath);
        }

        [Test]
        public void UpdatesConnectionStateAfterDatabaseWasLoadedSuccessfully()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            string receivedMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => receivedMessage = x.Notification);

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(null);

            // Assert
            receivedMessage.Should().Be(Resources.Message_UpdateConnectionStateForMainVM);
        }

        [Test]
        public void ShowMessageWhenDatabaseCanNotBeLoaded()
        {
            // Arrange
            this.mockRepository.Setup(x => x.LoadDatabase(It.IsAny<string>())).Throws(new Exception());
            this.mockDialogService.Setup(x => x.ShowDatabaseFileDialog()).Returns(DatabaseFactory.TestFilePath);

            // Act
            this.optionViewModel.LoadDatabaseCommand.Execute(this);

            // Assert
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Exception_Message_CouldNotLoadDatabase, It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}