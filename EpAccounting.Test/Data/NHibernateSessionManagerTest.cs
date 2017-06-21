// ///////////////////////////////////
// File: NHibernateSessionManagerTest.cs
// Last Change: 11.03.2017  13:28
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Data
{
    using System;
    using System.IO;
    using EpAccounting.Data;
    using FluentAssertions;
    using FluentNHibernate.Cfg;
    using NHibernate;
    using NUnit.Framework;



    [TestFixture]
    public class NHibernateSessionManagerTest
    {
        [SetUp]
        public void Init()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
        }

        [TearDown]
        public void CleanUp()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
        }

        [Test]
        public void ImplementsISessionManager()
        {
            // Assert
            typeof(NHibernateSessionManager).Should().Implement<ISessionManager>();
        }

        [Test]
        public void DerivedFromSessionManager()
        {
            // Assert
            typeof(NHibernateSessionManager).Should().BeDerivedFrom<SessionManager>();
        }

        [Test]
        public void NotConnectedAfterCreation()
        {
            // Act
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Assert
            sessionManager.IsConnected.Should().BeFalse();
        }

        [Test]
        public void EmptyFilePathAfterCreation()
        {
            // Act
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Assert
            sessionManager.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void CanCreateDatabase()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            FileAssert.Exists(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void IsConnectedAfterDatabaseCreation()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            sessionManager.IsConnected.Should().BeTrue();
        }

        [Test]
        public void SetFilePathWhenDatabaseWasCreated()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            sessionManager.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void ThrowExceptionIfInvalidFilePathForDatabaseCreation()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            Action action = () => sessionManager.CreateDatabase("Desktop\\Test");

            // Assert
            action.ShouldThrow<FluentConfigurationException>();
        }

        [Test]
        public void DoNotSetFilePathWhenDatabaseCreationThrewException()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            try
            {
                sessionManager.CreateDatabase("Desktop\\Test");
            }
            catch
            {
                // do nothing
            }

            // Assert
            sessionManager.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void CanLoadDatabase()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            FileAssert.Exists(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void IsConnectedAfterDatabaseLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            sessionManager.IsConnected.Should().BeTrue();
        }

        [Test]
        public void SetFilePathWhenDatabaseWasLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            sessionManager.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            sessionManager.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void ThrowExceptionIfInvalidFilePathForDatabaseLoading()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            Action action = () => sessionManager.LoadDatabase("Desktop\\Test");

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void ThrowExceptionIfInvalidFileShouldBeLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            string tempFilePath = DatabaseFactory.TestFolderPath + "\\Test.xlsx";
            FileStream fileStream = File.Create(tempFilePath);
            fileStream.Close();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            Action action = () => sessionManager.LoadDatabase(tempFilePath);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void DoNotSetFilePathWhenDatabaseLoadingThrewException()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            try
            {
                sessionManager.LoadDatabase("Desktop\\Test");
            }
            catch
            {
                // do nothing
            }

            // Assert
            sessionManager.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void DoNothingIfDatabaseShouldBeClosedWhenNotConnected()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            Action action = () => sessionManager.CloseDatabase();

            // Assert
            action.ShouldNotThrow<Exception>();
        }

        [Test]
        public void CloseDatabaseAndClearFilePath()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            sessionManager.CloseDatabase();

            // Assert
            sessionManager.IsConnected.Should().BeFalse();
            sessionManager.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void CanGetSessionWhenConnected()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();

            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            ISession session = sessionManager.OpenSession();

            // Assert
            session.Should().NotBeNull();
        }

        [Test]
        public void ThrowExceptionWhenSessionWillBeOpenedByNotConnectedDatabase()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();

            // Act
            Action action = () => sessionManager.OpenSession();

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }
    }
}