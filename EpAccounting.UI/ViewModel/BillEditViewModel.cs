// ///////////////////////////////////
// File: BillEditViewModel.cs
// Last Change: 19.02.2018, 19:40
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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


    public class BillEditViewModel : BindableViewModelBase, IDisposable
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

        private Bill _currentBill;
        private BillDetailViewModel _currentBillDetailViewModel;

        private IBillState _currentBillState;
        private BillEmptyState _billEmptyState;
        private BillSearchState _billSearchState;
        private BillCreationState _billCreationState;
        private BillLoadedState _billLoadedState;
        private BillEditState _billEditState;

        private RelayCommand _clearFieldsCommand;

        #endregion



        #region Constructors

        public BillEditViewModel(IRepository repository, IDialogService dialogService)
        {
            this._repository = repository;
            this._dialogService = dialogService;

            this.InitPropertyInfos();
            this.InitBillStateList();
            this.InitBillCommands();

            this._currentBillState = this.GetBillEmptyState();

            Messenger.Default.Register<NotificationMessage<Client>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public BillDetailViewModel CurrentBillDetailViewModel
        {
            get { return this._currentBillDetailViewModel; }
            private set { this.SetProperty(ref this._currentBillDetailViewModel, value); }
        }

        public IBillState CurrentBillState
        {
            get { return this._currentBillState; }
            private set { this.SetProperty(ref this._currentBillState, value); }
        }

        public bool CanInsertIDs
        {
            get { return this._repository.IsConnected && this.CurrentBillState is BillSearchState; }
        }

        public bool CanEditPrintedStatus
        {
            get
            {
                return this._repository.IsConnected &&
                       (this.CurrentBillState is BillSearchState || this.CurrentBillState is BillEditState);
            }
        }

        public bool CanEditData
        {
            get { return this._repository.IsConnected && this.CanCommit(); }
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

        public List<ImageCommandViewModel> BillCommands { get; private set; }

        private PropertyInfo[] PropertyInfos { get; set; }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }

        #endregion



        public virtual void ChangeToEmptyMode()
        {
            this.LoadBill(new Bill() { Client = new Client { CityToPostalCode = new CityToPostalCode() } });
            this.LoadBillState(this.GetBillEmptyState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToSearchMode()
        {
            this.LoadBill(new Bill() { Client = new Client { CityToPostalCode = new CityToPostalCode() } });
            this.LoadBillState(this.GetBillSearchState());
            this.UpdateViewModel();
        }

        public virtual void ChangeToLoadedMode(Bill bill = null)
        {
            this.LoadBill(bill);
            this.LoadBillState(this.GetBillLoadedState());
            this.UpdateViewModel();
            this.SendLoadBillItemEditViewModelMessage(this._currentBill);
        }

        public virtual void ChangeToEditMode()
        {
            this.LoadBillState(this.GetBillEditState());
            this.UpdateViewModel();
        }

        public virtual void Reload(int billId = -1)
        {
            try
            {
                if (billId == -1)
                {
                    billId = this._currentBill.Id;
                }

                this.ChangeToLoadedMode(this._repository.GetById<Bill>(billId));
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, "Could not reload bill!");
            }
        }

        public virtual async Task<bool> SaveOrUpdateBillAsync()
        {
            try
            {
                if (this.CurrentBillState == this.GetBillCreationState())
                {
                    this.CurrentBillDetailViewModel.Printed = false;
                }

                this._repository.SaveOrUpdate(this._currentBill);
                this.SendUpdateClientMessage();
                return true;
            }
            catch (Exception e)
            {
                await this._dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateBill, e.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteBillAsync()
        {
            bool shouldDelete = await this._dialogService.ShowDialogYesNo(Resources.Dialog_Title_Attention,
                                                                          Resources.Dialog_Question_DeleteBill);

            if (!shouldDelete)
            {
                return false;
            }

            try
            {
                this._repository.Delete(this._currentBill);
                this.SendRemoveBillMessage();
                return true;
            }
            catch (Exception e)
            {
                await this._dialogService.ShowMessage(Resources.Dialog_Title_CanNotDeleteBill, e.Message);
                return false;
            }
        }

        private void ChangeToCreationMode(int clientId)
        {
            Bill bill;

            try
            {
                bill = new Bill
                       {
                           Client = this._repository.GetById<Client>(clientId),
                           KindOfBill = KindOfBill.Rechnung,
                           KindOfVat = KindOfVat.inkl_MwSt,
                           VatPercentage = Settings.Default.VatPercentage,
                           Date = DateTime.Now.Date.ToShortDateString()
                       };
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, string.Format("Could not create bill for client with id = '{0}'", clientId));
                return;
            }

            this.LoadBill(bill);
            this.LoadBillState(this.GetBillCreationState());
            this.UpdateViewModel();
            this.SendLoadBillItemEditViewModelMessage(bill);
        }

        private void LoadBill(Bill bill)
        {
            if (bill == null)
            {
                return;
            }

            this.SetCurrentBill(bill);
        }

        private void SetCurrentBill(Bill bill)
        {
            this._currentBill = bill;
            this.CurrentBillDetailViewModel = new BillDetailViewModel(this._currentBill, this._repository, this._dialogService);
        }

        private void LoadBillState(IBillState billState)
        {
            this.CurrentBillState = billState;
        }

        private void UpdateViewModel()
        {
            this.UpdateCommands();
            this.UpdateProperties();
            this.SendEnableStateForBillItemEditing();
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel command in this.BillCommands)
            {
                command.RelayCommand.RaiseCanExecuteChanged();
            }

            this.ClearFieldsCommand.RaiseCanExecuteChanged();
        }

        private void UpdateProperties()
        {
            foreach (PropertyInfo propertyInfo in this.PropertyInfos)
            {
                this.RaisePropertyChanged(propertyInfo.Name);
            }
        }

        private void InsertClientData(Client messageContent)
        {
            this.CurrentBillDetailViewModel.ClientId = messageContent.Id;
            this.CurrentBillDetailViewModel.Title = messageContent.Title;
            this.CurrentBillDetailViewModel.FirstName = messageContent.FirstName;
            this.CurrentBillDetailViewModel.LastName = messageContent.LastName;
            this.CurrentBillDetailViewModel.Street = messageContent.Street;
            this.CurrentBillDetailViewModel.HouseNumber = messageContent.HouseNumber;
            this.CurrentBillDetailViewModel.PostalCode = messageContent.CityToPostalCode.PostalCode;
            this.CurrentBillDetailViewModel.City = messageContent.CityToPostalCode.City;
        }

        private Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> GetBillSearchCriterion()
        {
            Conjunction billConjunction = Restrictions.Conjunction();

            // Bill data
            if (this.CurrentBillDetailViewModel.Id != 0)
            {
                billConjunction.Add(Restrictions.Where<Bill>(b => b.Id == this.CurrentBillDetailViewModel.Id));
                return new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);
            }

            if (this.CurrentBillDetailViewModel.KindOfBill != null)
            {
                billConjunction.Add(Restrictions.Where<Bill>(b => b.KindOfBill == this.CurrentBillDetailViewModel.KindOfBill));
            }

            if (this.CurrentBillDetailViewModel.KindOfVat != null)
            {
                billConjunction.Add(Restrictions.Where<Bill>(b => b.KindOfVat == this.CurrentBillDetailViewModel.KindOfVat));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.Date))
            {
                billConjunction.Add(Restrictions.Where<Bill>(b => b.Date.IsLike(this.CurrentBillDetailViewModel.Date, MatchMode.Anywhere)));
            }

            if (this.CurrentBillDetailViewModel.Printed != null)
            {
                billConjunction.Add(Restrictions.Where<Bill>(b => b.Printed == this.CurrentBillDetailViewModel.Printed));
            }

            Conjunction clientConjunction = Restrictions.Conjunction();

            // Client data
            // if client id is passed, it should just search bills for this client
            if (this.CurrentBillDetailViewModel.ClientId != 0)
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.Id == this.CurrentBillDetailViewModel.ClientId));
                return new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);
            }

            // searches all bills with specific client data
            if (this.CurrentBillDetailViewModel.Title != null)
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == this.CurrentBillDetailViewModel.Title));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.CompanyName))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.CompanyName.IsLike(this.CurrentBillDetailViewModel.CompanyName, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.FirstName))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(this.CurrentBillDetailViewModel.FirstName, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.LastName))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(this.CurrentBillDetailViewModel.LastName, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.Street))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(this.CurrentBillDetailViewModel.Street, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.HouseNumber))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(this.CurrentBillDetailViewModel.HouseNumber, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.PostalCode))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.PostalCode.IsLike(this.CurrentBillDetailViewModel.PostalCode, MatchMode.Anywhere)));
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.City))
            {
                clientConjunction.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.City.IsLike(this.CurrentBillDetailViewModel.City, MatchMode.Anywhere)));
            }

            return new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);
        }

        private bool CanClearFields()
        {
            return this.CurrentBillState == this.GetBillLoadedState();
        }

        private void ClearFields()
        {
            this.ChangeToEmptyMode();
            Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillSearchViewModelMessageForBillVM));
        }

        private void InitPropertyInfos()
        {
            this.PropertyInfos = this.GetType()
                                     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(prop => prop.DeclaringType == this.GetType() &&
                                                    prop.CanRead && prop.GetMethod.IsPublic &&
                                                    prop.PropertyType != typeof(RelayCommand)).ToArray();
        }

        private void InitBillStateList()
        {
            this._billEmptyState = new BillEmptyState(this);
            this._billSearchState = new BillSearchState(this);
            this._billCreationState = new BillCreationState(this);
            this._billLoadedState = new BillLoadedState(this);
            this._billEditState = new BillEditState(this);
        }

        public IBillState GetBillEmptyState()
        {
            return this._billEmptyState;
        }

        public IBillState GetBillSearchState()
        {
            return this._billSearchState;
        }

        public IBillState GetBillCreationState()
        {
            return this._billCreationState;
        }

        public IBillState GetBillLoadedState()
        {
            return this._billLoadedState;
        }

        public IBillState GetBillEditState()
        {
            return this._billEditState;
        }

        private void ExecuteNotificationMessage(NotificationMessage<Client> message)
        {
            if (message.Notification == Resources.Message_SwitchToSearchModeAndLoadClientDataForBillEditVM)
            {
                this.CurrentBillState.SwitchToSearchMode();
                this.InsertClientData(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_LoadSelectedBillForBillEditVM)
            {
                this.Reload(message.Content);
            }
            else if (message.Notification == Resources.Message_CreateNewBillForBillEditVM)
            {
                this.ChangeToCreationMode(message.Content);
            }
            else if (message.Notification == Resources.Message_UpdateClientValuesForBillEditVM)
            {
                if (message.Content == this.CurrentBillDetailViewModel?.ClientId)
                {
                    this.Reload();
                }
            }
            else if (message.Notification == Resources.Message_RemoveClientForBillEditVM)
            {
                if (message.Content == this.CurrentBillDetailViewModel?.ClientId)
                {
                    this.ChangeToEmptyMode();
                }
            }
        }

        public virtual void SendBillSearchCriterionMessage()
        {
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion,
                                       Expression<Func<Bill, Client>>,
                                       ICriterion>>(this.GetBillSearchCriterion(),
                                                    Resources.Message_BillSearchCriteriaForBillSearchVM));
        }

        public virtual void SendUpdateBillValuesMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentBillDetailViewModel.Id,
                                                                Resources.Message_UpdateBillValuesMessageForBillSearchVM));
        }

        public virtual void SendRemoveBillMessage()
        {
            this.SendReloadBillSearchMessage();
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ResetBillItemEditVMAndChangeToSearchWorkspaceForBillVM));
        }

        public void SendReloadBillSearchMessage()
        {
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ReloadBillForBillSearchVM));
        }

        private void SendUpdateClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this._currentBill.Client.Id,
                                                                Resources.Message_UpdateClientForClientEditVM));
        }

        private void SendLoadBillItemEditViewModelMessage(Bill bill)
        {
            Messenger.Default.Send(new NotificationMessage<Bill>(bill,
                                                                 Resources.Message_LoadBillItemEditViewModelForBillVM));
        }

        private void SendEnableStateForBillItemEditing()
        {
            if (this.CurrentBillState == this.GetBillEditState() || this.CurrentBillState == this.GetBillCreationState())
            {
                Messenger.Default.Send(new NotificationMessage<bool>(true,
                                                                     Resources.Message_EnableStateForBillItemEditVM));
                Messenger.Default.Send(new NotificationMessage<bool>(false,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false,
                                                                     Resources.Message_EnableStateForBillItemEditVM));
                Messenger.Default.Send(new NotificationMessage<bool>(true,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
        }

        private void InitBillCommands()
        {
            this.BillCommands = new List<ImageCommandViewModel>()
                                {
                                    new ImageCommandViewModel(
                                                              Resources.img_search,
                                                              Resources.Command_DisplayName_Search,
                                                              new RelayCommand(this.SwitchToSearchMode, this.CanSwitchToSearchMode)),
                                    new ImageCommandViewModel(
                                                              Resources.img_edit,
                                                              Resources.Command_DisplayName_Edit,
                                                              new RelayCommand(this.SwitchToEditMode, this.CanSwitchToEditMode)),
                                    new ImageCommandViewModel(
                                                              Resources.img_saveOrUpdate,
                                                              Resources.Command_DisplayName_SaveOrUpdate,
                                                              new RelayCommand(this.Commit, this.CanCommit)),
                                    new ImageCommandViewModel(
                                                              Resources.img_cancel,
                                                              Resources.Command_DisplayName_Cancel,
                                                              new RelayCommand(this.Cancel, this.CanCancel)),
                                    new ImageCommandViewModel(
                                                              Resources.img_delete,
                                                              Resources.Command_DisplayName_Delete,
                                                              new RelayCommand(this.Delete, this.CanDelete))
                                };
        }

        private bool CanSwitchToSearchMode()
        {
            return this._repository.IsConnected && this.CurrentBillState.CanSwitchToSearchMode();
        }

        private void SwitchToSearchMode()
        {
            this.CurrentBillState.SwitchToSearchMode();
        }

        private bool CanSwitchToEditMode()
        {
            return this._repository.IsConnected && this.CurrentBillState.CanSwitchToEditMode();
        }

        private void SwitchToEditMode()
        {
            this.CurrentBillState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            return this._repository.IsConnected && this.CurrentBillState.CanCommit();
        }

        private void Commit()
        {
            this.CurrentBillState.Commit();
        }

        private bool CanCancel()
        {
            return this._repository.IsConnected && this.CurrentBillState.CanCancel();
        }

        private void Cancel()
        {
            this.CurrentBillState.Cancel();
        }

        private bool CanDelete()
        {
            return this._repository.IsConnected && this.CurrentBillState.CanDelete();
        }

        private void Delete()
        {
            this.CurrentBillState.Delete();
        }
    }
}