// ///////////////////////////////////
// File: BillItemEditViewModel.cs
// Last Change: 21.06.2017  15:22
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Messaging;



    public class BillItemEditViewModel : BillWorkspaceViewModel
    {
        #region Fields

        private Bill currentBill;
        private ObservableCollection<BillItemDetailViewModel> _billItemDetailViewModels;

        private bool _isEditingEnabled;

        #endregion



        #region Constructors / Destructor

        public BillItemEditViewModel()
        {
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public ObservableCollection<BillItemDetailViewModel> BillItemDetailViewModels
        {
            get
            {
                if (this._billItemDetailViewModels == null)
                {
                    this._billItemDetailViewModels = new ObservableCollection<BillItemDetailViewModel>();
                }
                return this._billItemDetailViewModels;
            }
        }

        public bool IsEditingEnabled
        {
            get { return this._isEditingEnabled; }
            private set { this.SetProperty(ref this._isEditingEnabled, value); }
        }

        #endregion



        public void LoadBill(Bill bill)
        {
            this.LoadBillItems(bill);
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Messenger_Message_EnableStateForBillItemEditing)
            {
                this.IsEditingEnabled = message.Content;
            }
        }

        private void LoadBillItems(Bill bill)
        {
            this.currentBill = bill;

            this.BillItemDetailViewModels.Clear();

            foreach (BillItem billItem in this.currentBill.BillItems)
            {
                this.BillItemDetailViewModels.Add(new BillItemDetailViewModel(billItem));
            }
        }
    }
}