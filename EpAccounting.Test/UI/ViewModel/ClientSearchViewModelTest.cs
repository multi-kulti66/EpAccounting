// ///////////////////////////////////
// File: ClientSearchViewModelTest.cs
// Last Change: 18.03.2017  21:49
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
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
        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            // Assert
            typeof(ClientSearchViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void GetEmptyListAfterCreation()
        {
            // Arrange
            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel();

            // Assert
            clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleClientToListThatMatchesCriterion()
        {
            // Arrange
            const string ExpectedFirstName = "Andre";
            const string ExpectedLastName = "Multerer";
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>()))
                          .Returns(new List<Client> { new Client { FirstName = ExpectedFirstName, LastName = ExpectedLastName } });

            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel(mockRepository);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Messenger_Message_ClientSearchCriteria));

            // Assert
            clientSearchViewModel.FoundClients.Should().HaveCount(1);
            clientSearchViewModel.FoundClients[0].FirstName.Should().Be(ExpectedFirstName);
            clientSearchViewModel.FoundClients[0].LastName.Should().Be(ExpectedLastName);
        }

        [Test]
        public void SelectedClientReturnsNullAfterCreation()
        {
            // Act
            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel();

            // Assert
            clientSearchViewModel.SelectedClientDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedClientChanges()
        {
            // Arrange
            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel();
            clientSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientSearchViewModel.SelectedClientDetailViewModel = new ClientDetailViewModel(new Client());

            // Assert
            clientSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedClientDetailViewModel);
        }

        [Test]
        public void UpdatesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const string PreviousFirstName = "Andre";
            const string ExpectedFirstName = "Stefanie";
            Client expectedClient = new Client { ClientId = 1, FirstName = ExpectedFirstName, LastName = "Multerer" };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);

            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel(mockRepository);
            clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { ClientId = 1, FirstName = PreviousFirstName, LastName = "Multerer" }));
            clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(new Client { ClientId = 2, FirstName = PreviousFirstName, LastName = "Multerer" }));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_UpdateClientValues));


            // Assert
            clientSearchViewModel.FoundClients.Should().HaveCount(2);
            clientSearchViewModel.FoundClients[0].FirstName.Should().Be(ExpectedFirstName);
            clientSearchViewModel.FoundClients[1].FirstName.Should().Be(PreviousFirstName);
        }

        [Test]
        public void RemovesClientWhenUpdateNotificationReceived()
        {
            // Arrange
            const int Id = 1;
            const string FirstName = "Stefanie";
            const string LastName = "Multerer";

            Client expectedClient = new Client { ClientId = Id, FirstName = FirstName, LastName = LastName };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(expectedClient);

            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel(mockRepository);
            clientSearchViewModel.FoundClients.Add(new ClientDetailViewModel(expectedClient));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_RemoveClient));


            // Assert
            clientSearchViewModel.FoundClients.Should().HaveCount(0);
        }

        [Test]
        public void EnableClientLoading()
        {
            // Arrange
            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateForClientLoading));

            // Assert
            clientSearchViewModel.IsClientLoadingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableClientLoading()
        {
            // Arrange
            ClientSearchViewModel clientSearchViewModel = this.GetDefaultClientSearchViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Messenger_Message_EnableStateForClientLoading));

            // Assert
            clientSearchViewModel.IsClientLoadingEnabled.Should().BeFalse();
        }

        private ClientSearchViewModel GetDefaultClientSearchViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            return new ClientSearchViewModel(mockRepository.Object);
        }

        private ClientSearchViewModel GetDefaultClientSearchViewModel(Mock<IRepository> repository)
        {
            return new ClientSearchViewModel(repository.Object);
        }
    }
}