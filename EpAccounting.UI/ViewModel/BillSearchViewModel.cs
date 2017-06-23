// ///////////////////////////////////
// File: BillSearchViewModel.cs
// Last Change: 04.05.2017  20:52
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class BillSearchViewModel : BillWorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;

        private ObservableCollection<BillDetailViewModel> _foundBills;
        private BillDetailViewModel _selectedBillDetailViewModel;

        #endregion



        #region Constructors / Destructor

        public BillSearchViewModel(IRepository repository)
        {
            this.repository = repository;

            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public ObservableCollection<BillDetailViewModel> FoundBills
        {
            get
            {
                if (this._foundBills == null)
                {
                    this._foundBills = new ObservableCollection<BillDetailViewModel>();
                }

                return this._foundBills;
            }
        }

        public BillDetailViewModel SelectedBillDetailViewModel
        {
            get { return this._selectedBillDetailViewModel; }
            set
            {
                this.SetProperty(ref this._selectedBillDetailViewModel, value);

                if (this._selectedBillDetailViewModel != null)
                {
                    this.SendLoadSelectedBillMessage();
                }
            }
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage<ICriterion> message)
        {
            if (message.Notification == Resources.Messenger_Message_BillSearchCriteria)
            {
                this.LoadSearchedBills(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_UpdateBillValues)
            {
                this.UpdateBillViaBillId(message.Content);
            }
            else if (message.Notification == Resources.Messenger_Message_UpdateClientValues)
            {
                this.UpdateBillViaClientId(message.Content);
            }
            else if (message.Notification == Resources.Messenger_Message_RemoveBill)
            {
                this.RemoveBill(message.Content);
            }
        }

        private void LoadSearchedBills(ICriterion criterion)
        {
            this.FoundBills.Clear();

            foreach (Bill bill in this.repository.GetByCriteria<Bill>(criterion).ToList())
            {
                this.FoundBills.Add(new BillDetailViewModel(bill));
            }
        }

        private void UpdateBillViaBillId(int id)
        {
            for (int i = 0; i < this.FoundBills.Count; i++)
            {
                if (this.FoundBills[i].BillId == id)
                {
                    Bill bill = this.repository.GetById<Bill>(id);
                    this.FoundBills[i] = new BillDetailViewModel(bill);
                }
            }
        }

        private void UpdateBillViaClientId(int id)
        {
            for (int i = 0; i < this.FoundBills.Count; i++)
            {
                if (this.FoundBills[i].ClientId == id)
                {
                    Bill bill = this.repository.GetById<Bill>(this.FoundBills[i].BillId);
                    this.FoundBills[i] = new BillDetailViewModel(bill);
                }
            }
        }

        private void RemoveBill(int id)
        {
            for (int i = 0; i < this.FoundBills.Count; i++)
            {
                if (this.FoundBills[i].BillId == id)
                {
                    this.FoundBills.RemoveAt(i);
                }
            }
        }

        private void SendLoadSelectedBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.SelectedBillDetailViewModel.BillId, Resources.Messenger_Message_LoadSelectedBill));
        }
    }
}