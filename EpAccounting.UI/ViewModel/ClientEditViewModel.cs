// ///////////////////////////////////
// File: ClientEditViewModel.cs
// Last Change: 29.08.2017  18:49
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class ClientEditViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private Client currentClient;
        private ClientDetailViewModel _currentClientDetailViewModel;

        private IClientState _currentClientState;
        private ClientEmptyState _clientEmptyState;
        private ClientSearchState _clientSearchState;
        private ClientCreationState _clientCreationState;
        private ClientLoadedState _clientLoadedState;
        private ClientEditState _clientEditState;

        private RelayCommand _createNewBillCommand;
        private RelayCommand _loadBillsFromClientCommand;
        private RelayCommand _clearFieldsCommand;

        #endregion



        #region Constructors / Destructor

        public ClientEditViewModel(IRepository repository, IDialogService dialogService)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.InitPropertyInfos();
            this.InitClientStates();
            this.InitClientCommands();

            this._currentClientState = this.GetClientEmptyState();
            this.SetCurrentClient(new Client());

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public ClientDetailViewModel CurrentClientDetailViewModel
        {
            get { return this._currentClientDetailViewModel; }
            private set { this.SetProperty(ref this._currentClientDetailViewModel, value); }
        }

        public IClientState CurrentClientState
        {
            get { return this._currentClientState; }
            private set { this.SetProperty(ref this._currentClientState, value); }
        }

        public bool CanInsertClientId
        {
            get
            {
                if (this.repository.IsConnected && this.CurrentClientState is ClientSearchState)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanEditClientData
        {
            get
            {
                if (this.repository.IsConnected && this.CanCommit())
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanLoadBills
        {
            get
            {
                if (this.repository.IsConnected && this.CurrentClientState is ClientLoadedState)
                {
                    return true;
                }

                return false;
            }
        }

        private PropertyInfo[] PropertyInfos { get; set; }

        #endregion



        public virtual void ChangeToEmptyMode()
        {
            this.LoadClient(new Client());
            this.LoadClientState(this.GetClientEmptyState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToCreationMode()
        {
            this.LoadClient(new Client());
            this.LoadClientState(this.GetClientCreationState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToSearchMode()
        {
            this.LoadClient(new Client());
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
            if (this.CurrentClientState != this.GetClientLoadedState())
            {
                return;
            }

            this.LoadClientState(this.GetClientEditState());
            this.UpdateViewModel();
        }

        public virtual void Reload()
        {
            this.ChangeToLoadedMode(this.repository.GetById<Client>(this.currentClient.ClientId));
        }

        public virtual async Task<bool> SaveOrUpdateClientAsync()
        {
            if (this.CurrentClientDetailViewModel.HasMissingLastName)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient, Resources.Dialog_Message_ClientHasMissingNamePart);
                return false;
            }

            try
            {
                bool shouldSaveOrUpdate = true;
                Client equalClient = this.GetEqualClient();

                if (equalClient != null)
                {
                    shouldSaveOrUpdate = await this.dialogService.ShowDialogYesNo(Resources.Dialog_Title_ClientExistsAlready,
                                                                                  string.Format(Resources.Dialog_Question_AddToExistingClient, equalClient));
                }

                if (shouldSaveOrUpdate)
                {
                    this.repository.SaveOrUpdate(this.currentClient);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient, e.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteClientAsync()
        {
            bool shouldDelete = await this.dialogService.ShowDialogYesNo(Resources.Dialog_Title_Attention,
                                                                         Resources.Dialog_Question_DeleteClient);

            if (!shouldDelete)
            {
                return false;
            }

            try
            {
                this.repository.Delete(this.currentClient);
                this.SendRemoveClientMessage();
                return true;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, e.Message);
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
            this.currentClient = client;
            this.CurrentClientDetailViewModel = new ClientDetailViewModel(this.currentClient);
        }

        private void LoadClientState(IClientState clientState)
        {
            this.CurrentClientState = clientState;
        }

        private void UpdateViewModel()
        {
            this.UpdateCommands();
            this.UpdateProperties();
            this.SendEnableStateForClientSearchAndWorkspaceMessage();
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel command in this.ClientCommands)
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

        private Client GetEqualClient()
        {
            return this.repository.GetByCriteria<Client>(this.GetEqualClientCriterion(), 1).FirstOrDefault();
        }

        private ICriterion GetEqualClientCriterion()
        {
            return Restrictions.Where<Client>(c => (c.ClientId != this.CurrentClientDetailViewModel.ClientId &&
                                                    c.FirstName == this.CurrentClientDetailViewModel.FirstName) &&
                                                   (c.LastName == this.CurrentClientDetailViewModel.LastName));
        }

        private ICriterion GetClientSearchCriterion()
        {
            Conjunction conjunction = Restrictions.Conjunction();

            if (this.CurrentClientDetailViewModel.ClientId != 0)
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.ClientId == this.CurrentClientDetailViewModel.ClientId));
                return conjunction;
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Title))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(this.CurrentClientDetailViewModel.Title, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.FirstName))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(this.CurrentClientDetailViewModel.FirstName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.LastName))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(this.CurrentClientDetailViewModel.LastName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Street))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(this.CurrentClientDetailViewModel.Street, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.HouseNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(this.CurrentClientDetailViewModel.HouseNumber, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PostalCode))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PostalCode.IsLike(this.CurrentClientDetailViewModel.PostalCode, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.City))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.City.IsLike(this.CurrentClientDetailViewModel.City, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.DateOfBirth))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.DateOfBirth.IsLike(this.CurrentClientDetailViewModel.DateOfBirth, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber1))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber1.IsLike(this.CurrentClientDetailViewModel.PhoneNumber1, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber2))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber2.IsLike(this.CurrentClientDetailViewModel.PhoneNumber2, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.MobileNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.MobileNumber.IsLike(this.CurrentClientDetailViewModel.MobileNumber, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Telefax))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Telefax.IsLike(this.CurrentClientDetailViewModel.Telefax, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Email))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Email.IsLike(this.CurrentClientDetailViewModel.Email, MatchMode.Anywhere)));
            }

            return conjunction;
        }

        private void InitPropertyInfos()
        {
            this.PropertyInfos = this.GetType()
                                     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(prop => prop.DeclaringType == this.GetType() &&
                                                    prop.CanRead && prop.GetMethod.IsPublic &&
                                                    prop.PropertyType != typeof(RelayCommand)).ToArray();
        }



        #region ClientState Methods

        private void InitClientStates()
        {
            this._clientEmptyState = new ClientEmptyState(this);
            this._clientSearchState = new ClientSearchState(this);
            this._clientCreationState = new ClientCreationState(this);
            this._clientLoadedState = new ClientLoadedState(this);
            this._clientEditState = new ClientEditState(this);
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

        #endregion



        #region Messenger

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_LoadClientForClientEditVM)
            {
                this.ChangeToLoadedMode(this.repository.GetById<Client>(message.Content));
            }
            else if (message.Notification == Resources.Message_UpdateClientForClientEditVM)
            {
                if (this.currentClient.ClientId == message.Content)
                {
                    this.Reload();
                }
            }
        }

        public virtual void SendClientSearchCriterionMessage()
        {
            Messenger.Default.Send(new NotificationMessage<ICriterion>(this.GetClientSearchCriterion(),
                                                                       Resources.Message_ClientSearchCriteriaForClientSearchVM));
        }

        public virtual void SendUpdateClientValuesMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_UpdateClientValuesForClientSearchVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_UpdateClientValuesForBillEditVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_UpdateClientValuesForBillSearchVM));
        }

        private void SendRemoveClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_RemoveClientForClientSearchVM));

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_RemoveClientForBillEditVM));

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_RemoveClientForBillSearchVM));

            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId,
                                                                Resources.Message_RemoveClientForBillItemEditVM));
        }

        private void SendEnableStateForClientSearchAndWorkspaceMessage()
        {
            if (this.CurrentClientState == this.GetClientEditState() || this.CurrentClientState == this.GetClientCreationState())
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

        #endregion



        #region Commands

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

        public RelayCommand ClearFieldsCommand
        {
            get {
                if (this._clearFieldsCommand == null)
                {
                    this._clearFieldsCommand = new RelayCommand(this.ClearFields, this.CanClearFields);
                }

                return this._clearFieldsCommand;
            }
        }

        private bool CanCreateNewBill()
        {
            if (this.CurrentClientState == this.GetClientLoadedState())
            {
                return true;
            }

            return false;
        }

        private void SendCreateNewBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Message_CreateNewBillForBillEditVM));
        }

        private bool CanSendLoadBillsFromClientMessage()
        {
            if (this.CurrentClientState == this.GetClientLoadedState())
            {
                return true;
            }

            return false;
        }

        private void SendLoadBillsFromClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ChangeToBillWorkspaceForMainVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.currentClient.ClientId, Resources.Message_LoadBillsFromClientForBillSearchVM));
            Messenger.Default.Send(new NotificationMessage<Client>(this.currentClient, Resources.Message_SwitchToSearchModeAndLoadClientDataForBillEditVM));
        }

        private bool CanClearFields()
        {
            if (this.CurrentClientState == this.GetClientLoadedState())
            {
                return true;
            }

            return false;
        }

        private void ClearFields()
        {
            this.ChangeToEmptyMode();
        }

        #endregion



        #region State Commands

        public List<ImageCommandViewModel> ClientCommands { get; private set; }

        private void InitClientCommands()
        {
            this.ClientCommands = new List<ImageCommandViewModel>
                                  {
                                      new ImageCommandViewModel(Resources.img_client_search,
                                                                Resources.Command_DisplayName_Search,
                                                                Resources.Command_Message_Client_Search,
                                                                new RelayCommand(this.SwitchToSearchMode, this.CanSwitchToSearchMode)),
                                      new ImageCommandViewModel(Resources.img_client_add,
                                                                Resources.Command_DisplayName_Add,
                                                                Resources.Command_Message_Client_Add,
                                                                new RelayCommand(this.SwitchToAddMode, this.CanSwitchToAddMode)),
                                      new ImageCommandViewModel(Resources.img_client_edit,
                                                                Resources.Command_DisplayName_Edit,
                                                                Resources.Command_Message_Client_Edit,
                                                                new RelayCommand(this.SwitchToEditMode, this.CanSwitchToEditMode)),
                                      new ImageCommandViewModel(Resources.img_client_saveOrUpdate,
                                                                Resources.Command_DisplayName_SaveOrUpdate,
                                                                Resources.Command_Message_Client_SaveOrUpdate,
                                                                new RelayCommand(this.Commit, this.CanCommit)),
                                      new ImageCommandViewModel(Resources.img_client_cancel,
                                                                Resources.Command_DisplayName_Cancel,
                                                                Resources.Command_Message_Client_Cancel,
                                                                new RelayCommand(this.Cancel, this.CanCancel)),
                                      new ImageCommandViewModel(Resources.img_delete,
                                                                Resources.Command_DisplayName_Delete,
                                                                Resources.Command_Message_Client_Delete,
                                                                new RelayCommand(this.Delete, this.CanDelete))
                                  };
        }

        private bool CanSwitchToSearchMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToSearchMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToSearchMode()
        {
            this.CurrentClientState.SwitchToSearchMode();
        }

        private bool CanSwitchToAddMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToAddMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToAddMode()
        {
            this.CurrentClientState.SwitchToAddMode();
        }

        private bool CanSwitchToEditMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToEditMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToEditMode()
        {
            this.CurrentClientState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanCommit())
            {
                return true;
            }

            return false;
        }

        private void Commit()
        {
            this.CurrentClientState.Commit();
        }

        private bool CanCancel()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanCancel())
            {
                return true;
            }

            return false;
        }

        private void Cancel()
        {
            this.CurrentClientState.Cancel();
        }

        private bool CanDelete()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanDelete())
            {
                return true;
            }

            return false;
        }

        private void Delete()
        {
            this.CurrentClientState.Delete();
        }

        #endregion
    }
}