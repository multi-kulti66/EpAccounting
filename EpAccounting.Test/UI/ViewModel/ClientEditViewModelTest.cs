// ///////////////////////////////////
// File: ClientEditViewModelTest.cs
// Last Change: 03.06.2017  20:18
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class ClientEditViewModelTest
    {
        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateClientEditViewModelObject()
        {
            // Act
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Assert
            clientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingCurrentClientDetailViewModelAfterCreation()
        {
            // Act
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Assert
            clientEditViewModel.CurrentClientDetailViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingClientStateAfterCreation()
        {
            // Act
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Assert
            clientEditViewModel.CurrentClientState.Should().NotBeNull().And.BeOfType<ClientEmptyState>();
        }

        [Test]
        public void CanInsertClientIdShouldReturnTrue()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientSearchState());

            // Assert
            clientEditViewModel.CanInsertClientId.Should().BeTrue();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenClientStateNotSearchState()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Assert
            clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientSearchState());

            // Assert
            clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanDoBillActionShouldReturnTrue()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.CanDoBillAction.Should().BeTrue();
        }

        [Test]
        public void CanDoBillActionShouldReturnFalseWhenClientStateNotLoadedState()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Assert
            clientEditViewModel.CanDoBillAction.Should().BeFalse();
        }

        [Test]
        public void CanDoBillActionShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.CanDoBillAction.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnTrue()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientSearchState());

            // Assert
            clientEditViewModel.CanEditClientData.Should().BeTrue();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenClientStateCanNotCommit()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Assert
            clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientSearchState());

            // Assert
            clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentClientDetailViewModel()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Act
            clientEditViewModel.Load(new Client { FirstName = "Andre" }, clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be("Andre");
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientDetailViewModelChanges()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientEditViewModel.Load(new Client { FirstName = "Andre" }, clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualClientDetailViewModelWillBeSet()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            Client client = new Client();
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisesPropertyChangedWhenClientWillBeReloaded()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);
            ClientEditViewModel clientEditViewModel = this.GetClientEditViewModelWithClientDetailViewModelValues(mockRepository, ModelFactory.DefaultClientFirstName, ModelFactory.DefaultClientLastName);
            clientEditViewModel.CurrentClientDetailViewModel.FirstName = "Alfred";
            clientEditViewModel.CurrentClientDetailViewModel.LastName = "Hugendubel";
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientEditViewModel.Reload();

            // Assert
            clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
            clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void CanChangeCurrentClientState()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Act
            clientEditViewModel.Load(new Client(), clientEditViewModel.GetClientCreationState());

            // Assert
            clientEditViewModel.CurrentClientState.Should().BeOfType<ClientCreationState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientStateChanges()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientEditViewModel.Load(new Client(), clientEditViewModel.GetClientCreationState());

            // Assert
            clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientState);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualClientStateWillBeSet()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            clientEditViewModel.Load(new Client(), clientEditViewModel.GetClientEmptyState());

            // Assert
            clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientState);
        }

        [Test]
        public void DoNotChangeClientAndClientStateWhenParametersAreNull()
        {
            // Arrange
            const string ExpectedFirstName = "Andre";
            const string ExpectedLastName = "Multerer";
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            clientEditViewModel.Load(new Client { FirstName = ExpectedFirstName, LastName = ExpectedLastName }, clientEditViewModel.GetClientLoadedState());

            // Act
            clientEditViewModel.Load(null, null);

            // Assert
            clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ExpectedFirstName);
            clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().Be(ExpectedLastName);
            clientEditViewModel.CurrentClientState.Should().Be(clientEditViewModel.GetClientLoadedState());
        }

        [Test]
        public void GetInstancesOfClientStates()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Act
            IClientState clientEmptyState = clientEditViewModel.GetClientEmptyState();
            IClientState clientCreationState = clientEditViewModel.GetClientCreationState();
            IClientState clientSearchState = clientEditViewModel.GetClientSearchState();
            IClientState clientLoadedState = clientEditViewModel.GetClientLoadedState();
            IClientState clientEditState = clientEditViewModel.GetClientEditState();

            // Assert
            clientEmptyState.Should().BeOfType<ClientEmptyState>();
            clientCreationState.Should().BeOfType<ClientCreationState>();
            clientSearchState.Should().BeOfType<ClientSearchState>();
            clientLoadedState.Should().BeOfType<ClientLoadedState>();
            clientEditState.Should().BeOfType<ClientEditState>();
        }

        [Test]
        public void ClientCommandsAreInitialized()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Assert
            clientEditViewModel.ClientCommands.Should().HaveCount(6);
        }

        [Test]
        public async Task DoNotAddNewClientWhenClientHasMissingNamePart()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockDialogService);

            // Act
            bool result = await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            mockDialogService.Verify(x => x.ShowMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfNoEqualClientExists()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Act
            bool result = await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            mockRepository.Verify(x => x.SaveOrUpdate(client), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfEqualClientExistsAndDialogResultIsYes()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client() });

            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(true));

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository, mockDialogService);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Act
            bool result = await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            mockRepository.Verify(x => x.SaveOrUpdate(client), Times.Once);
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotAddClientToDatabaseIfEqualClientExistsAndDialogResultIsNo()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client() });

            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository, mockDialogService);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Act
            bool result = await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            mockRepository.Verify(x => x.SaveOrUpdate(client), Times.Never);
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotRaisePropertyChangedWhenNewClientWasAdded()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());
            clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            clientEditViewModel.ShouldNotRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Client>())).Throws(new Exception());
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository, mockDialogService);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Act
            bool result = await clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            mockRepository.Verify(x => x.SaveOrUpdate(client), Times.Once);
            mockDialogService.Verify(x => x.ShowMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteClientWhenDialogResultIsNo()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockDialogService);

            // Act
            bool result = await clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteClientWhenDialogResultIsYes()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository, mockDialogService);

            // Act
            bool result = await clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeTrue();
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockRepository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.Delete(It.IsAny<Client>())).Throws(new Exception());
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository, mockDialogService);
            Client client = new Client() { FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            // Act
            bool result = await clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            mockRepository.Verify(x => x.Delete(client), Times.Once);
            mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesJustForASpecificClientId()
        {
            // Arrange
            const int ExpectedId = 2;
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            Client client = new Client { ClientId = ExpectedId, FirstName = "Andre", LastName = "Multerer" };
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            // Act
            clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(Restrictions.Conjunction().Add(Restrictions.Where<Client>(c => c.ClientId == ExpectedId)).ToString());
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesWithAllEnteredClientValues()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            Client client = ModelFactory.GetDefaultClient();
            clientEditViewModel.Load(client, clientEditViewModel.GetClientLoadedState());

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            Conjunction ExpectedConjunction = Restrictions.Conjunction();
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(ModelFactory.DefaultClientFirstName, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(ModelFactory.DefaultClientLastName, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(ModelFactory.DefaultClientStreet, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(ModelFactory.DefaultClientHouseNumber, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.PostalCode.IsLike(ModelFactory.DefaultClientPostalCode, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.City.IsLike(ModelFactory.DefaultClientCity, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.DateOfBirth.IsLike(ModelFactory.DefaultClientDateOfBirth, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber1.IsLike(ModelFactory.DefaultClientPhoneNumber1, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber2.IsLike(ModelFactory.DefaultClientPhoneNumber2, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.MobileNumber.IsLike(ModelFactory.DefaultClientMobileNumber, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.Telefax.IsLike(ModelFactory.DefaultClientTelefax, MatchMode.Anywhere)));
            ExpectedConjunction.Add(Restrictions.Where<Client>(c => c.Email.IsLike(ModelFactory.DefaultClientEmail, MatchMode.Anywhere)));

            // Act
            clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(ExpectedConjunction.ToString());
        }

        [Test]
        public void ExecutesCommandsWhenCanConditionsMet()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);

            Mock<IClientState> mockClientState = new Mock<IClientState>();
            mockClientState.Setup(x => x.CanSwitchToSearchMode()).Returns(true);
            mockClientState.Setup(x => x.CanSwitchToAddMode()).Returns(true);
            mockClientState.Setup(x => x.CanSwitchToEditMode()).Returns(true);
            mockClientState.Setup(x => x.CanCommit()).Returns(true);
            mockClientState.Setup(x => x.CanCancel()).Returns(true);
            mockClientState.Setup(x => x.CanDelete()).Returns(true);

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            clientEditViewModel.Load(new Client(), mockClientState.Object);

            // Act
            foreach (ImageCommandViewModel command in clientEditViewModel.ClientCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockClientState.Verify(x => x.SwitchToSearchMode(), Times.Once);
            mockClientState.Verify(x => x.SwitchToAddMode(), Times.Once);
            mockClientState.Verify(x => x.SwitchToEditMode(), Times.Once);
            mockClientState.Verify(x => x.Commit(), Times.Once);
            mockClientState.Verify(x => x.Cancel(), Times.Once);
            mockClientState.Verify(x => x.Delete(), Times.Once);
        }

        [Test]
        public void DoesNotExecuteCommandsWhenCanConditionsDoesNotMet()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);

            Mock<IClientState> mockClientState = new Mock<IClientState>();
            mockClientState.Setup(x => x.CanSwitchToSearchMode()).Returns(false);
            mockClientState.Setup(x => x.CanSwitchToAddMode()).Returns(false);
            mockClientState.Setup(x => x.CanSwitchToEditMode()).Returns(false);
            mockClientState.Setup(x => x.CanCommit()).Returns(false);
            mockClientState.Setup(x => x.CanCancel()).Returns(false);
            mockClientState.Setup(x => x.CanDelete()).Returns(false);

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            clientEditViewModel.Load(new Client(), mockClientState.Object);

            // Act
            foreach (ImageCommandViewModel command in clientEditViewModel.ClientCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockClientState.Verify(x => x.SwitchToSearchMode(), Times.Never);
            mockClientState.Verify(x => x.SwitchToAddMode(), Times.Never);
            mockClientState.Verify(x => x.SwitchToEditMode(), Times.Never);
            mockClientState.Verify(x => x.Commit(), Times.Never);
            mockClientState.Verify(x => x.Cancel(), Times.Never);
            mockClientState.Verify(x => x.Delete(), Times.Never);
        }

        [Test]
        public void DoesNotExecuteCommandsWhenRepositoryNotInitialized()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);

            Mock<IClientState> mockClientState = new Mock<IClientState>();
            mockClientState.Setup(x => x.CanSwitchToSearchMode()).Returns(true);
            mockClientState.Setup(x => x.CanSwitchToAddMode()).Returns(true);
            mockClientState.Setup(x => x.CanSwitchToEditMode()).Returns(true);
            mockClientState.Setup(x => x.CanCommit()).Returns(true);
            mockClientState.Setup(x => x.CanCancel()).Returns(true);
            mockClientState.Setup(x => x.CanDelete()).Returns(true);

            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            clientEditViewModel.Load(new Client(), mockClientState.Object);

            // Act
            foreach (ImageCommandViewModel command in clientEditViewModel.ClientCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockClientState.Verify(x => x.SwitchToSearchMode(), Times.Never);
            mockClientState.Verify(x => x.SwitchToAddMode(), Times.Never);
            mockClientState.Verify(x => x.SwitchToEditMode(), Times.Never);
            mockClientState.Verify(x => x.Commit(), Times.Never);
            mockClientState.Verify(x => x.Cancel(), Times.Never);
            mockClientState.Verify(x => x.Delete(), Times.Never);
        }

        [Test]
        public void LoadClientViaMessengerMessage()
        {
            // Arrange
            const int ExpectedId = 2;
            Mock<IRepository> mockRepsoitory = new Mock<IRepository>();
            mockRepsoitory.Setup(x => x.GetById<Client>(ExpectedId)).Returns(new Client());
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepsoitory);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(ExpectedId, Resources.Messenger_Message_LoadSelectedClientMessageForClientEditVM));

            // Assert
            mockRepsoitory.Verify(x => x.GetById<Client>(ExpectedId), Times.Once);
        }

        [Test]
        public void SendsUpdateClientValuesMessage()
        {
            // Arrange
            const int ExpectedId = 10;
            int clientId = 0;
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            clientEditViewModel.Load(new Client { ClientId = ExpectedId }, clientEditViewModel.GetClientLoadedState());
            Messenger.Default.Register<NotificationMessage<int>>(this, x => clientId = x.Content);

            // Act
            clientEditViewModel.SendUpdateClientValuesMessage();

            // Assert
            clientId.Should().Be(ExpectedId);
        }

        [Test]
        public void SendChangeClientLoadingStateMessageWithTrueValueWhenNewClientOrClientStateWillBeLoaded()
        {
            // Arrange
            bool isEnabled = false;
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => isEnabled = x.Content);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientLoadedState());

            // Assert
            isEnabled.Should().BeTrue();
        }

        [Test]
        public void SendChangeClientLoadingStateMessageWithFalseValueWhenNewClientOrClientStateWillBeLoaded()
        {
            // Arrange
            bool isEnabled = true;
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => isEnabled = x.Content);

            // Act
            clientEditViewModel.Load(null, clientEditViewModel.GetClientEditState());

            // Assert
            isEnabled.Should().BeFalse();
        }

        [Test]
        public void CanNotSendCreateNewBillWhenNotInLoadedClientState()
        {
            // Act
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Assert
            clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanSendCreateNewBillWhenNotInLoadedClientState()
        {
            // Arrange
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Act
            clientEditViewModel.Load(ModelFactory.GetDefaultClient(), clientEditViewModel.GetClientLoadedState());

            // Assert
            clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendCreateNewBillMessage()
        {
            // Arrange
            string notificationMessage = this.ToString();
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessage = x.Notification);
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();

            // Act
            clientEditViewModel.Load(ModelFactory.GetDefaultClient(), clientEditViewModel.GetClientLoadedState());
            clientEditViewModel.CreateNewBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Messenger_Message_CreateNewBillMessageForBillEditVM);
        }

        #endregion



        private ClientEditViewModel GetDefaultClientEditViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new ClientEditViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private ClientEditViewModel GetDefaultClientEditViewModel(Mock<IDialogService> dialogService)
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();

            return new ClientEditViewModel(mockRepository.Object, dialogService.Object);
        }

        private ClientEditViewModel GetDefaultClientEditViewModel(Mock<IRepository> repository)
        {
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new ClientEditViewModel(repository.Object, mockDialogService.Object);
        }

        private ClientEditViewModel GetDefaultClientEditViewModel(Mock<IRepository> repository, Mock<IDialogService> dialogService)
        {
            return new ClientEditViewModel(repository.Object, dialogService.Object);
        }

        private ClientEditViewModel GetClientEditViewModelWithClientDetailViewModelValues(Mock<IRepository> mockRepository, string firstName, string lastName)
        {
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel(mockRepository);
            clientEditViewModel.Load(new Client { FirstName = firstName, LastName = lastName }, clientEditViewModel.GetClientLoadedState());

            return clientEditViewModel;
        }

        private ClientEditViewModel GetClientEditViewModelWithClientDetailViewModelValues(string firstName, string lastName)
        {
            ClientEditViewModel clientEditViewModel = this.GetDefaultClientEditViewModel();
            clientEditViewModel.Load(new Client { FirstName = firstName, LastName = lastName }, clientEditViewModel.GetClientLoadedState());

            return clientEditViewModel;
        }
    }
}