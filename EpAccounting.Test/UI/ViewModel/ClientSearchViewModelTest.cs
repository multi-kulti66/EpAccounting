// ///////////////////////////////////
// File: ClientSearchViewModelTest.cs
// Last Change: 22.10.2017  15:57
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
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class ClientSearchViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private ClientSearchViewModel _clientSearchViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._clientSearchViewModel = new ClientSearchViewModel(this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._clientSearchViewModel = null;
            GC.Collect();
        }


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
            this._clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleClientToListThatMatchesCriterion()
        {
            // Arrange
            const string expectedFirstName = "Andre";
            const string expectedLastName = "Multerer";
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client> { new Client { FirstName = expectedFirstName, LastName = expectedLastName } });

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.FoundClients.Should().HaveCount(1);
            this._clientSearchViewModel.FoundClients[0].FirstName.Should().Be(expectedFirstName);
            this._clientSearchViewModel.FoundClients[0].LastName.Should().Be(expectedLastName);
        }

        [Test]
        public void ShowMessageWhenSearchedClientsCouldNotBeLoaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Throws<Exception>();

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SetsNumberOfPageAndCurrentPageWhenSingleClientIsLoadedViaCriteria()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(1);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(1);
            this._clientSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        [Test]
        public void SetsNumberOfPageAndCurrentPageWhenClientsAreLoadedViaCriteria()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(1);
            this._clientSearchViewModel.NumberOfAllPages.Should().Be(2);
        }

        [Test]
        public void SelectedClientReturnsNullAfterCreation()
        {
            // Assert
            this._clientSearchViewModel.SelectedClientDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedClientChanges()
        {
            // Arrange
            this._clientSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(new Client(), this._mockRepository.Object);

            // Assert
            this._clientSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedClientDetailViewModel);
        }

        [Test]
        public void UpdatesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const string previousFirstName = "Andre";
            const string expectedFirstName = "Stefanie";
            Client expectedClient = new Client { Id = 1, FirstName = expectedFirstName, LastName = "Multerer", CityToPostalCode = ModelFactory.GetDefaultCityToPostalCode() };
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);

            this._clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { Id = 1, FirstName = previousFirstName, LastName = "Multerer", CityToPostalCode = ModelFactory.GetDefaultCityToPostalCode() }, this._mockRepository.Object));
            this._clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { Id = 2, FirstName = previousFirstName, LastName = "Multerer", CityToPostalCode = ModelFactory.GetDefaultCityToPostalCode() }, this._mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientValuesForClientSearchVM));

            // Assert
            this._clientSearchViewModel.FoundClients.Should().HaveCount(2);
            this._clientSearchViewModel.FoundClients[0].FirstName.Should().Be(expectedFirstName);
            this._clientSearchViewModel.FoundClients[1].FirstName.Should().Be(previousFirstName);
        }

        [Test]
        public void ShowMessageWhenClientsCouldNotBeUpdatedWhenNotificationReceived()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Throws<Exception>();

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientValuesForClientSearchVM));
            
            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdatesClientsCityReferenceWhenUpdateNotificationReceived()
        {
            // Arrange
            const string previousFirstName = "Andre";
            const string expectedFirstName = "Stefanie";
            Client expectedClient = new Client { Id = 1, FirstName = expectedFirstName, LastName = "Multerer", CityToPostalCode = ModelFactory.GetDefaultCityToPostalCode() };
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);

            this._clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client
                                                                                  {
                                                                                      Id = 1,
                                                                                      FirstName = previousFirstName,
                                                                                      LastName = "Multerer",
                                                                                      CityToPostalCode = ModelFactory.GetDefaultCityToPostalCode()
                                                                                  }, this._mockRepository.Object));
            this._clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client
                                                                                  {
                                                                                      Id = 2,
                                                                                      FirstName = previousFirstName,
                                                                                      LastName = "Multerer",
                                                                                      CityToPostalCode = new CityToPostalCode()
                                                                                                         {
                                                                                                             PostalCode = ModelFactory.DefaultCityToPostalCodePostalCode,
                                                                                                             City = "Amsterdam"
                                                                                                         }
                                                                                  }, this._mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientValuesForClientSearchVM));

            // Assert
            this._clientSearchViewModel.FoundClients.Should().HaveCount(2);
            this._clientSearchViewModel.FoundClients[0].City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
            this._clientSearchViewModel.FoundClients[1].City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void RemovesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const int id = 1;
            const string firstName = "Stefanie";
            const string lastName = "Multerer";

            Client expectedClient = new Client { Id = id, FirstName = firstName, LastName = lastName };
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>())).Returns(new List<Client>());

            this._clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(expectedClient, this._mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ReloadClientsForClientSearchVM));

            // Assert
            this._clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void EnableClientLoading()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForClientSearchVM));

            // Assert
            this._clientSearchViewModel.IsClientLoadingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableClientLoading()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_EnableStateForClientSearchVM));

            // Assert
            this._clientSearchViewModel.IsClientLoadingEnabled.Should().BeFalse();
        }

        [Test]
        public void CanNotLoadClient()
        {
            // Assert
            this._clientSearchViewModel.LoadSelectedClientCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadClient()
        {
            // Act
            this._clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(ModelFactory.GetDefaultClient(), this._mockRepository.Object);

            // Assert
            this._clientSearchViewModel.LoadSelectedClientCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendsLoadClientMessage()
        {
            // Arrange
            this._clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(ModelFactory.GetDefaultClient(), this._mockRepository.Object);
            string message = null;

            Messenger.Default.Register<NotificationMessage<int>>(this, x => message = x.Notification);

            // Act
            this._clientSearchViewModel.LoadSelectedClientCommand.Execute(null);

            // Assert
            message.Should().Be(Resources.Message_LoadClientForClientEditVM);
        }

        [Test]
        public void CanNotLoadNextPageWhenJustOnePage()
        {
            // Arrange
            const int numberOfElements = 10;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadNextPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsNextPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(2);
        }

        [Test]
        public void CanLoadLastPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.LoadLastPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsLastPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadLastPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(Convert.ToInt32(Math.Ceiling((double)numberOfElements / Settings.Default.PageSize)));
        }

        [Test]
        public void CanNotLoadPreviousPageWhenOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadPreviousPageWhenNotOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsPreviousPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadPreviousPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void CanNotLoadFirstPageWhenOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.LoadFirstPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadFirstPageWhenNotOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.LoadFirstPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsFirstPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._clientSearchViewModel.LoadFirstPageCommand.RelayCommand.Execute(null);

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void ChangesToPreviousPageWhenClientWasDeletedAndNoClientLeftOnPage()
        {
            // Arrange
            const int numberOfElements = 51;

            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));
            this._clientSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(numberOfElements - 1);

            // Act

            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_ClientSearchCriteriaForClientSearchVM));

            // Assert
            this._clientSearchViewModel.CurrentPage.Should().Be(1);
            this._clientSearchViewModel.NumberOfAllPages.Should().Be(1);
        }
    }
}