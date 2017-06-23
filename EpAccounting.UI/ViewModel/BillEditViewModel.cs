// ///////////////////////////////////
// File: BillEditViewModel.cs
// Last Change: 21.06.2017  16:35
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



    public class BillEditViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private Bill currentBill;
        private BillDetailViewModel _currentBillDetailViewModel;
        private IBillState _currentBillState;

        #endregion



        #region Constructors / Destructor

        public BillEditViewModel(IRepository repository, IDialogService dialogService)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.InitPropertyInfos();
            this.InitBillStateList();
            this.InitBillCommands();

            this._currentBillState = this.GetBillEmptyState();

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public List<ImageCommandViewModel> BillCommands { get; private set; }

        public IBillState CurrentBillState
        {
            get { return this._currentBillState; }
            private set { this.SetProperty(ref this._currentBillState, value); }
        }

        public BillDetailViewModel CurrentBillDetailViewModel
        {
            get { return this._currentBillDetailViewModel; }
            private set { this.SetProperty(ref this._currentBillDetailViewModel, value); }
        }

        public bool IsInSearchMode
        {
            get
            {
                if (this.repository.IsConnected && this.CurrentBillState is BillSearchState)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanEditBillData
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

        private List<IBillState> BillStates { get; set; }

        private PropertyInfo[] PropertyInfos { get; set; }

        #endregion



        public virtual void Load(Bill bill, IBillState billState)
        {
            this.LoadBill(bill);
            this.LoadBillState(billState);
            this.UpdateCommands();
            this.UpdateProperties();
            this.SendEnableStateForBillItemEditing();

            if (this.CurrentBillState == this.GetBillLoadedState())
            {
                Messenger.Default.Send(new NotificationMessage<Bill>(this.currentBill, Resources.Messenger_Message_LoadBillItemEditViewModel));
            }
        }

        public virtual void Reload()
        {
            this.Load(this.repository.GetById<Bill>(this.currentBill.BillId), this.GetBillLoadedState());
        }

        public virtual async Task<bool> SaveOrUpdateBillAsync()
        {
            try
            {
                this.repository.SaveOrUpdate(this.currentBill);
                return true;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateBill, e.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteBillAsync()
        {
            bool shouldDelete = await this.dialogService.ShowDialogYesNo(
                                                                         Resources.Dialog_Title_Attention,
                                                                         Resources.Dialog_Question_DeleteBill);

            if (!shouldDelete)
            {
                return false;
            }

            try
            {
                this.SendRemoveBillMessage();
                this.repository.Delete(this.currentBill);
                return true;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotDeleteBill, e.Message);
                return false;
            }
        }

        public virtual void SendBillSearchCriterionMessage()
        {
            Messenger.Default.Send(new NotificationMessage<ICriterion>(this.GetBillSearchCriterion(), Resources.Messenger_Message_BillSearchCriteria));
        }

        public virtual void SendUpdateBillValuesMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentBillDetailViewModel.BillId, Resources.Messenger_Message_UpdateBillValues));
        }

        public virtual void SendRemoveBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentBillDetailViewModel.BillId, Resources.Messenger_Message_RemoveBill));
        }

        private void SendEnableStateForBillItemEditing()
        {
            if (this.CurrentBillState.GetType() == typeof(BillEditState))
            {
                Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateForBillItemEditing));
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Messenger_Message_EnableStateForBillItemEditing));
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_LoadSelectedBill)
            {
                this.Load(this.repository.GetById<Bill>(message.Content), this.GetBillLoadedState());
            }
            if (message.Notification == Resources.Messenger_Message_UpdateClientValues)
            {
                this.Reload();
            }
        }

        private void LoadBill(Bill bill)
        {
            if (bill == null)
            {
                return;
            }

            this.SetCurrentBill(bill);
        }

        private void LoadBillState(IBillState billState)
        {
            if (billState == null)
            {
                return;
            }

            this.CurrentBillState = billState;
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel command in this.BillCommands)
            {
                command.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        private void UpdateProperties()
        {
            foreach (PropertyInfo propertyInfo in this.PropertyInfos)
            {
                this.RaisePropertyChanged(propertyInfo.Name);
            }
        }

        private void SetCurrentBill(Bill bill)
        {
            this.currentBill = bill;
            this.CurrentBillDetailViewModel = new BillDetailViewModel(this.currentBill);
        }

        private ICriterion GetBillSearchCriterion()
        {
            Conjunction conjunction = Restrictions.Conjunction();

            // Bill data
            if (this.CurrentBillDetailViewModel.BillId != 0)
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.BillId == this.CurrentBillDetailViewModel.BillId));
                return conjunction;
            }

            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.KindOfBill))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.KindOfBill.IsLike(this.CurrentBillDetailViewModel.KindOfBill, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.KindOfVat))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.KindOfVat.IsLike(this.CurrentBillDetailViewModel.KindOfVat, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.Date))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Date.IsLike(this.CurrentBillDetailViewModel.Date, MatchMode.Anywhere)));
            }

            // Client data
            // if client id is passed, it should just search bills for this client
            if (this.CurrentBillDetailViewModel.ClientId != 0)
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.ClientId == this.CurrentBillDetailViewModel.ClientId));
                return conjunction;
            }

            // searches all bills with specific client data
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.Title))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.Title.IsLike(this.CurrentBillDetailViewModel.Title, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.FirstName))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.FirstName.IsLike(this.CurrentBillDetailViewModel.FirstName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.LastName))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.LastName.IsLike(this.CurrentBillDetailViewModel.LastName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.Street))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.Street.IsLike(this.CurrentBillDetailViewModel.Street, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.HouseNumber))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.HouseNumber.IsLike(this.CurrentBillDetailViewModel.HouseNumber, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.PostalCode))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.PostalCode.IsLike(this.CurrentBillDetailViewModel.PostalCode, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentBillDetailViewModel.City))
            {
                conjunction.Add(Restrictions.Where<Bill>(b => b.Client.City.IsLike(this.CurrentBillDetailViewModel.City, MatchMode.Anywhere)));
            }

            return conjunction;
        }

        private void InitPropertyInfos()
        {
            this.PropertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                                                                                                                 prop => prop.DeclaringType == this.GetType() &&
                                                                                                                         prop.CanRead && prop.GetMethod.IsPublic &&
                                                                                                                         prop.PropertyType != typeof(RelayCommand)).ToArray();
        }



        #region BillState Methods

        private void InitBillStateList()
        {
            this.BillStates = new List<IBillState>
                              {
                                  new BillEmptyState(this),
                                  new BillSearchState(this),
                                  new BillCreationState(this),
                                  new BillLoadedState(this),
                                  new BillEditState(this)
                              };
        }

        public IBillState GetBillEmptyState()
        {
            return this.BillStates.Find(x => x.GetType() == typeof(BillEmptyState));
        }

        public IBillState GetBillSearchState()
        {
            return this.BillStates.Find(x => x.GetType() == typeof(BillSearchState));
        }

        public IBillState GetBillCreationState()
        {
            return this.BillStates.Find(x => x.GetType() == typeof(BillCreationState));
        }

        public IBillState GetBillLoadedState()
        {
            return this.BillStates.Find(x => x.GetType() == typeof(BillLoadedState));
        }

        public IBillState GetBillEditState()
        {
            return this.BillStates.Find(x => x.GetType() == typeof(BillEditState));
        }

        #endregion



        #region Command Methods

        private void InitBillCommands()
        {
            this.BillCommands = new List<ImageCommandViewModel>()
                                {
                                    new ImageCommandViewModel(
                                                              Resources.img_search,
                                                              Resources.Command_DisplayName_Search,
                                                              Resources.Command_Message_Bill_Search,
                                                              new RelayCommand(this.SwitchToSearchMode, this.CanSwitchToSearchMode)),
                                    new ImageCommandViewModel(
                                                              Resources.img_edit,
                                                              Resources.Command_DisplayName_Edit,
                                                              Resources.Command_Message_Bill_Edit,
                                                              new RelayCommand(this.SwitchToEditMode, this.CanSwitchToEditMode)),
                                    new ImageCommandViewModel(
                                                              Resources.img_saveOrUpdate,
                                                              Resources.Command_DisplayName_SaveOrUpdate,
                                                              Resources.Command_Message_Bill_SaveOrUpdate,
                                                              new RelayCommand(this.Commit, this.CanCommit)),
                                    new ImageCommandViewModel(
                                                              Resources.img_cancel,
                                                              Resources.Command_DisplayName_Cancel,
                                                              Resources.Command_Message_Bill_Cancel,
                                                              new RelayCommand(this.Cancel, this.CanCancel)),
                                    new ImageCommandViewModel(
                                                              Resources.img_delete,
                                                              Resources.Command_DisplayName_Delete,
                                                              Resources.Command_Message_Bill_Delete,
                                                              new RelayCommand(this.Delete, this.CanDelete))
                                };
        }

        private bool CanSwitchToSearchMode()
        {
            if (this.repository.IsConnected && this.CurrentBillState.CanSwitchToSearchMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToSearchMode()
        {
            this.CurrentBillState.SwitchToSearchMode();
        }

        private bool CanSwitchToEditMode()
        {
            if (this.repository.IsConnected && this.CurrentBillState.CanSwitchToEditMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToEditMode()
        {
            this.CurrentBillState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            if (this.repository.IsConnected && this.CurrentBillState.CanCommit())
            {
                return true;
            }

            return false;
        }

        private void Commit()
        {
            this.CurrentBillState.Commit();
        }

        private bool CanCancel()
        {
            if (this.repository.IsConnected && this.CurrentBillState.CanCancel())
            {
                return true;
            }

            return false;
        }

        private void Cancel()
        {
            this.CurrentBillState.Cancel();
        }

        private bool CanDelete()
        {
            if (this.repository.IsConnected && this.CurrentBillState.CanDelete())
            {
                return true;
            }

            return false;
        }

        private void Delete()
        {
            this.CurrentBillState.Delete();
        }

        #endregion
    }
}