// ///////////////////////////////////
// File: BillItemEditViewModel.cs
// Last Change: 23.06.2017  21:19
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;



    public class BillItemEditViewModel : BillWorkspaceViewModel
    {
        #region Fields

        private Bill currentBill;
        private ObservableCollection<BillItemDetailViewModel> _billItemDetailViewModels;
        private BillItemDetailViewModel _selectedBillItemDetailViewModel;

        private bool _isEditingEnabled;

        #endregion



        #region Constructors / Destructor

        public BillItemEditViewModel()
        {
            this.InitCommands();

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

        public BillItemDetailViewModel SelectedBillItemDetailViewModel
        {
            get { return this._selectedBillItemDetailViewModel; }
            set { this.SetProperty(ref this._selectedBillItemDetailViewModel, value); }
        }

        public List<ImageCommandViewModel> Commands { get; private set; }

        public bool IsEditingEnabled
        {
            get { return this._isEditingEnabled; }
            private set
            {
                this.SetProperty(ref this._isEditingEnabled, value);
                this.UpdateCommands();
            }
        }

        #endregion



        public void LoadBill(Bill bill)
        {
            this.LoadBillItems(bill);
        }

        public void Clear()
        {
            this.currentBill = null;
            this.BillItemDetailViewModels.Clear();
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



        #region Command Methods

        private void InitCommands()
        {
            this.Commands = new List<ImageCommandViewModel>
                            {
                                new ImageCommandViewModel(Resources.img_add,
                                                          Resources.Command_DisplayName_Add,
                                                          Resources.Command_Message_BillItem_Add,
                                                          new RelayCommand(this.AddItem, this.CanAddItem)),
                                new ImageCommandViewModel(Resources.img_remove,
                                                          Resources.Command_DisplayName_Delete,
                                                          Resources.Command_Message_BillItem_Delete,
                                                          new RelayCommand(this.DeleteItem, this.CanDeleteItem))
                            };
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel imageCommandViewModel in this.Commands)
            {
                imageCommandViewModel.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanAddItem()
        {
            return this.IsEditingEnabled;
        }

        private void AddItem()
        {
            BillItem billItem = new BillItem() {Position = this.BillItemDetailViewModels.Count + 1};
            BillItemDetailViewModel billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            this.currentBill.AddBillItem(billItem);
            this.BillItemDetailViewModels.Add(billItemDetailViewModel);
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_FocusBillItems));
        }

        private bool CanDeleteItem()
        {
            return this.IsEditingEnabled;
        }

        private void DeleteItem()
        {
            // TODO: implement
        }

        #endregion
    }
}