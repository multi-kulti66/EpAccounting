// ///////////////////////////////////
// File: BillItemEditViewModel.cs
// Last Change: 22.07.2017  19:38
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using EpAccounting.Model;
    using EpAccounting.UI.Markup;
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

        private ImageCommandViewModel _addItemCommand;
        private ImageCommandViewModel _deleteItemCommand;
        private ImageCommandViewModel _moveItemUpCommand;
        private ImageCommandViewModel _moveItemDownCommand;

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
            set
            {
                this.SetProperty(ref this._selectedBillItemDetailViewModel, value);
                this.UpdateCommands();
            }
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
            if (message.Notification == Resources.Messenger_Message_EnableStateMessageForBillItemEditVM)
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
                this.BillItemDetailViewModels.InsertOrderedBy(new BillItemDetailViewModel(billItem), x => x.Position);
            }
        }



        #region Command Methods

        private void InitCommands()
        {
            this.Commands = new List<ImageCommandViewModel>
                            {
                                this.MoveItemUpCommand,
                                this.MoveItemDownCommand,
                                this.AddItemCommand,
                                this.DeleteItemCommand
                            };
        }

        public ImageCommandViewModel MoveItemUpCommand
        {
            get
            {
                if (this._moveItemUpCommand == null)
                {
                    this._moveItemUpCommand = new ImageCommandViewModel(Resources.img_arrow_up,
                                                                        Resources.Command_DisplayName_Up,
                                                                        Resources.Command_Message_BillItem_Up,
                                                                        new RelayCommand(this.MoveItemUp, this.CanMoveItemUp));
                }

                return this._moveItemUpCommand;
            }
        }

        private bool CanMoveItemUp()
        {
            if (this.IsEditingEnabled && this.SelectedBillItemDetailViewModel != null && this.SelectedBillItemDetailViewModel.Position > 1)
            {
                return true;
            }

            return false;
        }

        private void MoveItemUp()
        {
            int oldIndex = this.SelectedBillItemDetailViewModel.Position - 1;
            int newIndex = oldIndex - 1;

            BillItemDetailViewModel tempItemDetailViewModel = this.BillItemDetailViewModels[oldIndex];
            this.BillItemDetailViewModels[oldIndex] = this.BillItemDetailViewModels[newIndex];
            this.BillItemDetailViewModels[newIndex] = tempItemDetailViewModel;

            this.BillItemDetailViewModels[oldIndex].Position = oldIndex + 1;
            this.BillItemDetailViewModels[newIndex].Position = newIndex + 1;

            this.SelectedBillItemDetailViewModel = this.BillItemDetailViewModels[newIndex];
        }

        public ImageCommandViewModel MoveItemDownCommand
        {
            get
            {
                if (this._moveItemDownCommand == null)
                {
                    this._moveItemDownCommand = new ImageCommandViewModel(Resources.img_arrow_down,
                                                                          Resources.Command_DisplayName_Down,
                                                                          Resources.Command_Message_BillItem_Down,
                                                                          new RelayCommand(this.MoveItemDown, this.CanMoveItemDown));
                }

                return this._moveItemDownCommand;
            }
        }

        private bool CanMoveItemDown()
        {
            if (this.IsEditingEnabled && this.SelectedBillItemDetailViewModel != null && this.SelectedBillItemDetailViewModel.Position < this.BillItemDetailViewModels.Count)
            {
                return true;
            }

            return false;
        }

        private void MoveItemDown()
        {
            int oldIndex = this.SelectedBillItemDetailViewModel.Position - 1;
            int newIndex = oldIndex + 1;

            BillItemDetailViewModel tempItemDetailViewModel = this.BillItemDetailViewModels[oldIndex];
            this.BillItemDetailViewModels[oldIndex] = this.BillItemDetailViewModels[newIndex];
            this.BillItemDetailViewModels[newIndex] = tempItemDetailViewModel;

            this.BillItemDetailViewModels[oldIndex].Position = oldIndex + 1;
            this.BillItemDetailViewModels[newIndex].Position = newIndex + 1;

            this.SelectedBillItemDetailViewModel = this.BillItemDetailViewModels[newIndex];
        }

        public ImageCommandViewModel AddItemCommand
        {
            get
            {
                if (this._addItemCommand == null)
                {
                    this._addItemCommand = new ImageCommandViewModel(Resources.img_add,
                                                                     Resources.Command_DisplayName_Add,
                                                                     Resources.Command_Message_BillItem_Add,
                                                                     new RelayCommand(this.AddItem, this.CanAddItem));
                }

                return this._addItemCommand;
            }
        }

        private bool CanAddItem()
        {
            return this.IsEditingEnabled;
        }

        private void AddItem()
        {
            BillItem billItem = new BillItem() { Position = this.BillItemDetailViewModels.Count + 1 };
            BillItemDetailViewModel billItemDetailViewModel = new BillItemDetailViewModel(billItem);

            this.currentBill.AddBillItem(billItem);
            this.BillItemDetailViewModels.Add(billItemDetailViewModel);
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_FocusBillItemsMessageForBillItemEditView));
        }

        public ImageCommandViewModel DeleteItemCommand
        {
            get
            {
                if (this._deleteItemCommand == null)
                {
                    this._deleteItemCommand = new ImageCommandViewModel(Resources.img_remove,
                                                                        Resources.Command_DisplayName_Delete,
                                                                        Resources.Command_Message_BillItem_Delete,
                                                                        new RelayCommand(this.DeleteItem, this.CanDeleteItem));
                }

                return this._deleteItemCommand;
            }
        }

        private bool CanDeleteItem()
        {
            if (this.IsEditingEnabled && this.SelectedBillItemDetailViewModel != null)
            {
                return true;
            }

            return false;
        }

        private void DeleteItem()
        {
            this.currentBill.BillItems.Remove(this.currentBill.BillItems.First(x => x.Id == this.SelectedBillItemDetailViewModel.Id));
            this.BillItemDetailViewModels.Remove(this.BillItemDetailViewModels.First(x => x.Id == this.SelectedBillItemDetailViewModel.Id));
            this.SelectedBillItemDetailViewModel = null;
            this.UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < this.BillItemDetailViewModels.Count; i++)
            {
                int currentPosition = i + 1;

                if (this.BillItemDetailViewModels[i].Position != currentPosition)
                {
                    this.BillItemDetailViewModels[i].Position = currentPosition;
                }
            }
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel imageCommandViewModel in this.Commands)
            {
                imageCommandViewModel.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}