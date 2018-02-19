// ///////////////////////////////////
// File: ClientEditViewModel.cs
// Last Change: 19.02.2018, 19:59
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using Model.Enum;
    using NHibernate.Criterion;
    using Properties;
    using Service;
    using State;


    public class ClientEditViewModel : BindableViewModelBase, IDisposable
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly IRepository _repository;

        private Client _currentClient;
        private IClientState _currentState;
        private ClientDetailViewModel _currentClientDetailViewModel;

        private ClientCreationState _clientCreationState;
        private ClientEditState _clientEditState;
        private ClientEmptyState _clientEmptyState;
        private ClientLoadedState _clientLoadedState;
        private ClientSearchState _clientSearchState;

        private RelayCommand _createNewBillCommand;
        private RelayCommand _clearFieldsCommand;
        private RelayCommand _loadBillsFromClientCommand;

        #endregion



        #region Constructors

        public ClientEditViewModel(IRepository repository, IDialogService dialogService)
        {
            this._repository = repository;
            this._dialogService = dialogService;

            this.InitPropertyInfos();
            this.InitStates();
            this.InitStateCommands();

            this._currentState = this.GetClientEmptyState();
            this.SetCurrentClient(new Client { CityToPostalCode = new CityToPostalCode() });

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public ClientDetailViewModel CurrentClientDetailViewModel
        {
            get { return this._currentClientDetailViewModel; }
            private set { this.SetProperty(ref this._currentClientDetailViewModel, value); }
        }

        public IClientState CurrentState
        {
            get { return this._currentState; }
            private set { this.SetProperty(ref this._currentState, value); }
        }

        public bool CanInsertClientId
        {
            get { return this._repository.IsConnected && this.CurrentState is ClientSearchState; }
        }

        public bool CanEditClientData
        {
            get { return this._repository.IsConnected && this.CanCommit(); }
        }

        public bool CanEditCompanyName
        {
            get
            {
                return this._repository.IsConnected && this.CanCommit() &&
                       (this._currentClient.Title == ClientTitle.Firma || this._currentState == this.GetClientSearchState());
            }
        }

        public bool CanLoadBills
        {
            get { return this._repository.IsConnected && this.CurrentState is ClientLoadedState; }
        }

        public RelayCommand ClearFieldsCommand
        {
            get
            {
                if (this._clearFieldsCommand == null)
                {
                    this._clearFieldsCommand = new RelayCommand(this.ClearFields, this.CanClearFields);
                }

                return this._clearFieldsCommand;
            }
        }

        public RelayCommand CreateNewBillCommand
        {
            get
            {
                if (this._createNewBillCommand == null)
                {
                    this._createNewBillCommand = new RelayCommand(this.SendCreateNewBillMessage, this.CanCreateNewBill);
                }

                return this._createNewBillCommand;
            }
        }

        public RelayCommand LoadBillsFromClientCommand
        {
            get
            {
                if (this._loadBillsFromClientCommand == null)
                {
                    this._loadBillsFromClientCommand = new RelayCommand(this.SendLoadBillsFromClientMessage, this.CanSendLoadBillsFromClientMessage);
                }

                return this._loadBillsFromClientCommand;
            }
        }

        public List<ImageCommandViewModel> StateCommands { get; private set; }

        private PropertyInfo[] PropertyInfos { get; set; }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }

        #endregion



        public virtual void Load(int clientId = -1)
        {
            try
            {
                // reload client when no id was given
                if (clientId == -1)
                {
                    clientId = this._currentClient.Id;
                }

                this.ChangeToLoadedMode(this._repository.GetById<Client>(clientId));
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, "Could not reload client!");
            }
        }

        public virtual async Task<bool> SaveOrUpdateClientAsync()
        {
            if (this.CurrentClientDetailViewModel.HasMissingValues)
            {
                await this._dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                      Resources.Dialog_Message_ClientHasMissingValues);
                return false;
            }

            try
            {
                bool shouldSaveOrUpdate = true;
                Client equalClient = this.GetEqualClient();

                if (equalClient != null)
                {
                    shouldSaveOrUpdate =
                        await this._dialogService.ShowDialogYesNo(Resources.Dialog_Title_ClientExistsAlready,
                                                                  string.Format(Resources.Dialog_Question_AddToExistingClient,
                                                                                equalClient));
                }

                if (!shouldSaveOrUpdate)
                {
                    return false;
                }

                Client tempClient = this._repository.GetById<Client>(this._currentClient.Id);
                this._repository.SaveOrUpdate(this._currentClient);

                if (tempClient != null)
                {
                    this.DeletePostalCodeIfNecessary(tempClient.CityToPostalCode);
                }

                return true;
            }
            catch (Exception e)
            {
                await this._dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient, e.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteClientAsync()
        {
            var shouldDelete = await this._dialogService.ShowDialogYesNo(Resources.Dialog_Title_Attention,
                                                                         Resources.Dialog_Question_DeleteClient);

            if (!shouldDelete)
            {
                return false;
            }

            try
            {
                CityToPostalCode oldCityToPostalCode = this._currentClient.CityToPostalCode;
                this._repository.Delete(this._currentClient);
                this.DeletePostalCodeIfNecessary(oldCityToPostalCode);
                this.SendRemoveClientMessage();
                return true;
            }
            catch (Exception e)
            {
                await this._dialogService.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, e.Message);
                return false;
            }
        }

        private void LoadClient(Client client)
        {
            if (client == null)
            {
                return;
            }

            this.SetCurrentClient(client);
        }

        private void SetCurrentClient(Client client)
        {
            this._currentClient = client;
            this.CurrentClientDetailViewModel = new ClientDetailViewModel(this._currentClient, this._repository);
        }

        private void LoadClientState(IClientState clientState)
        {
            this.CurrentState = clientState;
        }

        private void UpdateViewModel()
        {
            this.UpdateCommands();
            this.UpdateProperties();
            this.SendEnableStateForClientSearchAndWorkspaceMessage();
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel command in this.StateCommands)
            {
                command.RelayCommand.RaiseCanExecuteChanged();
            }

            this.CreateNewBillCommand.RaiseCanExecuteChanged();
            this.LoadBillsFromClientCommand.RaiseCanExecuteChanged();
            this.ClearFieldsCommand.RaiseCanExecuteChanged();
        }

        private void UpdateProperties()
        {
            foreach (PropertyInfo propertyInfo in this.PropertyInfos)
            {
                this.RaisePropertyChanged(propertyInfo.Name);
            }
        }

        /// <summary>
        /// Returns an equal client when one exists in database, otherwise null.
        /// </summary>
        /// <returns>equal client</returns>
        /// <exception cref="Exception">Thrown when Client could not be loaded from database.</exception>
        private Client GetEqualClient()
        {
            return this._repository.GetByCriteria<Client>(this.GetEqualClientCriterion(), 1).FirstOrDefault();
        }

        private ICriterion GetEqualClientCriterion()
        {
            return Restrictions.Where<Client>(c => c.Id != this.CurrentClientDetailViewModel.Id &&
                                                   c.FirstName == this.CurrentClientDetailViewModel.FirstName &&
                                                   c.LastName == this.CurrentClientDetailViewModel.LastName);
        }

        private ICriterion GetClientSearchCriterion()
        {
            Conjunction conjunction = Restrictions.Conjunction();

            if (this.CurrentClientDetailViewModel.Id != 0)
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Id == this.CurrentClientDetailViewModel.Id));
                return conjunction;
            }

            if (this.CurrentClientDetailViewModel.Title != null)
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Title == this.CurrentClientDetailViewModel.Title));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.CompanyName))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.CompanyName
                                                                .IsLike(this.CurrentClientDetailViewModel.CompanyName,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.FirstName))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.FirstName
                                                                .IsLike(this.CurrentClientDetailViewModel.FirstName,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.LastName))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.LastName
                                                                .IsLike(this.CurrentClientDetailViewModel.LastName,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Street))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.Street.IsLike(this.CurrentClientDetailViewModel.Street,
                                                                               MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.HouseNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.HouseNumber
                                                                .IsLike(this.CurrentClientDetailViewModel.HouseNumber,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PostalCode))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.CityToPostalCode.PostalCode
                                                                .IsLike(this.CurrentClientDetailViewModel.PostalCode,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.City))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.CityToPostalCode.City
                                                                .IsLike(this.CurrentClientDetailViewModel.City,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.DateOfBirth))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.DateOfBirth
                                                                .IsLike(this.CurrentClientDetailViewModel.DateOfBirth,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber1))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.PhoneNumber1
                                                                .IsLike(this.CurrentClientDetailViewModel.PhoneNumber1,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber2))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.PhoneNumber2
                                                                .IsLike(this.CurrentClientDetailViewModel.PhoneNumber2,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.MobileNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.MobileNumber
                                                                .IsLike(this.CurrentClientDetailViewModel.MobileNumber,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Telefax))
            {
                conjunction.Add(Restrictions.Where<Client>(c =>
                                                               c.Telefax
                                                                .IsLike(this.CurrentClientDetailViewModel.Telefax,
                                                                        MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Email))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Email.IsLike(this.CurrentClientDetailViewModel.Email,
                                                                               MatchMode.Anywhere)));
            }

            return conjunction;
        }

        /// <summary>
        /// Deletes the previous postal code - city combination when no other client possesses it.
        /// </summary>
        /// <exception cref="Exception">Thrown when no database connection or postal code could not be deleted.</exception>
        private void DeletePostalCodeIfNecessary(CityToPostalCode cityToPostalCode)
        {
            if (cityToPostalCode == null || string.IsNullOrEmpty(cityToPostalCode.PostalCode))
            {
                return;
            }

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.PostalCode == cityToPostalCode.PostalCode));

            int references = this._repository.GetQuantityByCriteria<Client>(conjunction);

            if (references == 0)
            {
                this._repository.Delete(cityToPostalCode);
            }
        }

        private void InitPropertyInfos()
        {
            this.PropertyInfos = this.GetType()
                                     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(prop => prop.DeclaringType == this.GetType() &&
                                                    prop.CanRead && prop.GetMethod.IsPublic &&
                                                    prop.PropertyType != typeof(RelayCommand)).ToArray();
        }

        public virtual void ChangeToEmptyMode()
        {
            this.LoadClient(new Client { CityToPostalCode = new CityToPostalCode() });
            this.LoadClientState(this.GetClientEmptyState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToCreationMode()
        {
            this.LoadClient(new Client { CityToPostalCode = new CityToPostalCode() });
            this.LoadClientState(this.GetClientCreationState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToSearchMode()
        {
            this.LoadClient(new Client { CityToPostalCode = new CityToPostalCode() });
            this.LoadClientState(this.GetClientSearchState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToLoadedMode(Client client = null)
        {
            this.LoadClient(client);
            this.LoadClientState(this.GetClientLoadedState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToEditMode()
        {
            if (this.CurrentState != this.GetClientLoadedState())
            {
                return;
            }

            this.LoadClientState(this.GetClientEditState());
            this.UpdateViewModel();
        }

        public IClientState GetClientEmptyState()
        {
            return this._clientEmptyState;
        }

        public IClientState GetClientSearchState()
        {
            return this._clientSearchState;
        }

        public IClientState GetClientCreationState()
        {
            return this._clientCreationState;
        }

        public IClientState GetClientLoadedState()
        {
            return this._clientLoadedState;
        }

        public IClientState GetClientEditState()
        {
            return this._clientEditState;
        }

        private void InitStates()
        {
            this._clientEmptyState = new ClientEmptyState(this);
            this._clientSearchState = new ClientSearchState(this);
            this._clientCreationState = new ClientCreationState(this);
            this._clientLoadedState = new ClientLoadedState(this);
            this._clientEditState = new ClientEditState(this);
        }

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_UpdateCompanyNameEnableStateForClientEditVM)
            {
                this.RaisePropertyChanged(() => this.CanEditCompanyName);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_LoadClientForClientEditVM)
            {
                this.Load(message.Content);
            }
            else if (message.Notification == Resources.Message_UpdateClientForClientEditVM)
            {
                if (this._currentClient.Id == message.Content)
                {
                    this.Load();
                }
            }
        }

        public virtual void SendClientSearchCriterionMessage()
        {
            Messenger.Default.Send(new NotificationMessage<ICriterion>(this.GetClientSearchCriterion(),
                                                                       Resources
                                                                          .Message_ClientSearchCriteriaForClientSearchVM));
        }

        public virtual void SendUpdateClientValuesMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_UpdateClientValuesForClientSearchVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_UpdateClientValuesForBillEditVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_UpdateClientValuesForBillSearchVM));
        }

        private void SendRemoveClientMessage()
        {
            this.SendReloadClientSearchMessage();

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_RemoveClientForBillEditVM));

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_RemoveClientForBillSearchVM));

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_RemoveClientForBillItemEditVM));
        }

        public void SendReloadClientSearchMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ReloadClientsForClientSearchVM));
        }

        private void SendEnableStateForClientSearchAndWorkspaceMessage()
        {
            if (this.CurrentState == this.GetClientEditState() || this.CurrentState == this.GetClientCreationState())
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false,
                                                                     Resources.Message_EnableStateForClientSearchVM));
                Messenger.Default.Send(new NotificationMessage<bool>(false,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<bool>(true,
                                                                     Resources.Message_EnableStateForClientSearchVM));
                Messenger.Default.Send(new NotificationMessage<bool>(true,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
        }

        private bool CanCreateNewBill()
        {
            return this.CurrentState == this.GetClientLoadedState();
        }

        private void SendCreateNewBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.Id,
                                                                Resources.Message_CreateNewBillForBillEditVM));
        }

        private bool CanSendLoadBillsFromClientMessage()
        {
            return this.CurrentState == this.GetClientLoadedState();
        }

        private void SendLoadBillsFromClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));
            Messenger.Default.Send(new NotificationMessage<int>(this._currentClient.Id,
                                                                Resources.Message_LoadBillsFromClientForBillSearchVM));
            Messenger.Default.Send(new NotificationMessage<Client>(this._currentClient,
                                                                   Resources
                                                                      .Message_SwitchToSearchModeAndLoadClientDataForBillEditVM));
        }

        private bool CanClearFields()
        {
            return this.CurrentState == this.GetClientLoadedState();
        }

        private void ClearFields()
        {
            this.ChangeToEmptyMode();
        }

        private void InitStateCommands()
        {
            this.StateCommands = new List<ImageCommandViewModel>
                                 {
                                     new ImageCommandViewModel(Resources.img_client_search,
                                                               Resources.Command_DisplayName_Search,
                                                               new RelayCommand(this.SwitchToSearchMode,
                                                                                this.CanSwitchToSearchMode)),
                                     new ImageCommandViewModel(Resources.img_client_add,
                                                               Resources.Command_DisplayName_Add,
                                                               new RelayCommand(this.SwitchToAddMode,
                                                                                this.CanSwitchToAddMode)),
                                     new ImageCommandViewModel(Resources.img_client_edit,
                                                               Resources.Command_DisplayName_Edit,
                                                               new RelayCommand(this.SwitchToEditMode,
                                                                                this.CanSwitchToEditMode)),
                                     new ImageCommandViewModel(Resources.img_client_saveOrUpdate,
                                                               Resources.Command_DisplayName_SaveOrUpdate,
                                                               new RelayCommand(this.Commit, this.CanCommit)),
                                     new ImageCommandViewModel(Resources.img_client_cancel,
                                                               Resources.Command_DisplayName_Cancel,
                                                               new RelayCommand(this.Cancel, this.CanCancel)),
                                     new ImageCommandViewModel(Resources.img_delete,
                                                               Resources.Command_DisplayName_Delete,
                                                               new RelayCommand(this.Delete, this.CanDelete))
                                 };
        }

        private bool CanSwitchToSearchMode()
        {
            return this._repository.IsConnected && this.CurrentState.CanSwitchToSearchMode();
        }

        private void SwitchToSearchMode()
        {
            this.CurrentState.SwitchToSearchMode();
        }

        private bool CanSwitchToAddMode()
        {
            return this._repository.IsConnected && this.CurrentState.CanSwitchToAddMode();
        }

        private void SwitchToAddMode()
        {
            this.CurrentState.SwitchToAddMode();
        }

        private bool CanSwitchToEditMode()
        {
            return this._repository.IsConnected && this.CurrentState.CanSwitchToEditMode();
        }

        private void SwitchToEditMode()
        {
            this.CurrentState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            return this._repository.IsConnected && this.CurrentState.CanCommit();
        }

        private void Commit()
        {
            this.CurrentState.Commit();
        }

        private bool CanCancel()
        {
            return this._repository.IsConnected && this.CurrentState.CanCancel();
        }

        private void Cancel()
        {
            this.CurrentState.Cancel();
        }

        private bool CanDelete()
        {
            return this._repository.IsConnected && this.CurrentState.CanDelete();
        }

        private void Delete()
        {
            this.CurrentState.Delete();
        }
    }
}