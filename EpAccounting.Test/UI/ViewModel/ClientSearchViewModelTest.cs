// ///////////////////////////////////
// File: ClientSearchViewModelTest.cs
// Last Change: 23.08.2017  20:33
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class ClientSearchViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private ClientSearchViewModel clientSearchViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.clientSearchViewModel = new ClientSearchViewModel(this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.clientSearchViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            // Assert
            typeof(ClientSearchViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void GetEmptyListAfterCreation()
        {
            // Assert
            this.clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleClientToListThatMatchesCriterion()
        {
            // Arrange
            const string ExpectedFirstName = "Andre";
            const string ExpectedLastName = "Multerer";
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client> { new Client { FirstName = ExpectedFirstName, LastName = ExpectedLastName } });

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.FoundClients.Should().HaveCount(1);
            this.clientSearchViewModel.FoundClients[0].FirstName.Should().Be(ExpectedFirstName);
            this.clientSearchViewModel.FoundClients[0].LastName.Should().Be(ExpectedLastName);
        }

        [Test]
        public void SetsNumberOfPageAndCurrentPageWhenSingleClientIsLoadedViaCriteria()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(1);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.CurrentPage.Should().Be(1);
            this.clientSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        [Test]
        public void SetsNumberOfPageAndCurrentPageWhenClientsAreLoadedViaCriteria()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.CurrentPage.Should().Be(1);
            this.clientSearchViewModel.NumberOfAllPages.Should().Be(2);
        }

        [Test]
        public void SelectedClientReturnsNullAfterCreation()
        {
            // Assert
            this.clientSearchViewModel.SelectedClientDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedClientChanges()
        {
            // Arrange
            this.clientSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(new Client());

            // Assert
            this.clientSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedClientDetailViewModel);
        }

        [Test]
        public void UpdatesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const string PreviousFirstName = "Andre";
            const string ExpectedFirstName = "Stefanie";
            Client expectedClient = new Client { Id = 1, FirstName = ExpectedFirstName, LastName = "Multerer" };
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);

            this.clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { Id = 1, FirstName = PreviousFirstName, LastName = "Multerer" }));
            this.clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { Id = 2, FirstName = PreviousFirstName, LastName = "Multerer" }));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientValuesForClientSearchVM));

            // Assert
            this.clientSearchViewModel.FoundClients.Should().HaveCount(2);
            this.clientSearchViewModel.FoundClients[0].FirstName.Should().Be(ExpectedFirstName);
            this.clientSearchViewModel.FoundClients[1].FirstName.Should().Be(PreviousFirstName);
        }

        [Test]
        public void RemovesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const int Id = 1;
            const string FirstName = "Stefanie";
            const string LastName = "Multerer";

            Client expectedClient = new Client { Id = Id, FirstName = FirstName, LastName = LastName };
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>())).Returns(new List<Client>());

            this.clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(expectedClient));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_RemoveClientForClientSearchVM));

            // Assert
            this.clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void EnableClientLoading()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForClientSearchVM));

            // Assert
            this.clientSearchViewModel.IsClientLoadingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableClientLoading()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_EnableStateForClientSearchVM));

            // Assert
            this.clientSearchViewModel.IsClientLoadingEnabled.Should().BeFalse();
        }

        [Test]
        public void CanNotLoadClient()
        {
            // Assert
            this.clientSearchViewModel.LoadSelectedClientCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadClient()
        {
            // Act
            this.clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(ModelFactory.GetDefaultClient());

            // Assert
            this.clientSearchViewModel.LoadSelectedClientCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendsLoadClientMessage()
        {
            // Arrange
            this.clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(ModelFactory.GetDefaultClient());
            string message = null;

            Messenger.Default.Register<NotificationMessage<int>>(this, x => message = x.Notification);

            // Act
            this.clientSearchViewModel.LoadSelectedClientCommand.Execute(null);

            // Assert
            message.Should().Be(Resources.Message_LoadClientForClientEditVM);
        }

        [Test]
        public void CanNotLoadNextPageWhenJustOnePage()
        {
            // Arrange
            const int NumberOfElements = 10;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this.clientSearchViewModel.CurrentPage.Should().Be(2);
        }

        [Test]
        public void CanNotLoadPreviousPageWhenOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadPreviousPageWhenNotOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this.clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsPreviousPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this.clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.Execute(null);

            // Assert
            this.clientSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void ChangesToPreviousPageWhenClientWasDeletedAndNoClientLeftOnPage()
        {
            // Arrange
            const int NumberOfElements = 51;

            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this.clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(NumberOfElements - 1);

            // Act

            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this.clientSearchViewModel.CurrentPage.Should().Be(1);
            this.clientSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        #endregion
    }
}