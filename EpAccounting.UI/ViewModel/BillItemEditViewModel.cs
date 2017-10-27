// ///////////////////////////////////
// File: BillItemEditViewModel.cs
// Last Change: 26.10.2017  21:50
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Markup;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;



    public class BillItemEditViewModel : BillWorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;
        private readonly IWordService wordSerivce;

        private Bill currentBill;
        private ObservableCollection<BillItemDetailViewModel> _billItemDetailViewModels;
        private BillItemDetailViewModel _selectedBillItemDetailViewModel;

        private bool _isEditingEnabled;
        private bool _isCreatingBill;

        private ImageCommandViewModel _addItemCommand;
        private ImageCommandViewModel _deleteItemCommand;
        private ImageCommandViewModel _moveItemUpCommand;
        private ImageCommandViewModel _moveItemDownCommand;

        private ImageCommandViewModel _createDocumentCommand;
        private ImageCommandViewModel _printDocumentCommand;

        private RelayCommand _changeVATCommand;

        private BackgroundWorker createBillBackgroundWorker;

        #endregion



        #region Constructors / Destructor

        public BillItemEditViewModel(IRepository repository, IDialogService dialogSerivce, IWordService wordSerivce)
        {
            this.repository = repository;
            this.dialogService = dialogSerivce;
            this.wordSerivce = wordSerivce;

            this.InitCommands();
            this.InitBackgroundWorker();

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);

            this.BillItemDetailViewModels.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion



        #region Properties

        public BillDetailViewModel CurrentBillDetailViewModel { get; private set; }

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

        public bool IsCreatingBill
        {
            get { return this._isCreatingBill; }
            private set { this.SetProperty(ref this._isCreatingBill, value); }
        }

        public bool CanChangeVAT { get; private set; }

        public bool IsEditingEnabled
        {
            get { return this._isEditingEnabled; }
            private set
            {
                this.SetProperty(ref this._isEditingEnabled, value);
                this.UpdateCommands();
            }
        }

        public decimal NettoSum { get; private set; }

        public decimal VatSum { get; private set; }

        public decimal BruttoSum { get; private set; }

        public RelayCommand ChangeVATCommand
        {
            get
            {
                if (this._changeVATCommand == null)
                {
                    this._changeVATCommand = new RelayCommand(this.ChangeVAT);
                }

                return this._changeVATCommand;
            }
        }

        #endregion



        public void Clear()
        {
            this.CurrentBillDetailViewModel = null;
            this.currentBill = null;
            this.BillItemDetailViewModels.Clear();
        }

        public void LoadBill(Bill bill)
        {
            this.CurrentBillDetailViewModel = new BillDetailViewModel(bill, this.repository);
            this.LoadBillItems(bill);
        }

        private void LoadBillItems(Bill bill)
        {
            this.currentBill = bill;

            this.BillItemDetailViewModels.Clear();

            foreach (BillItem billItem in this.currentBill.BillItems)
            {
                this.BillItemDetailViewModels.InsertOrderedBy(new BillItemDetailViewModel(billItem, this.repository), x => x.Position);
            }

            this.CalculateSums();
        }

        private void AdjustSinglePricesToKindOfVat()
        {
            if (this.currentBill != null)
            {
                foreach (BillItemDetailViewModel billItemDetailViewModel in this.BillItemDetailViewModels)
                {
                    if (this.CurrentBillDetailViewModel.KindOfVat == KindOfVat.inkl_MwSt)
                    {
                        billItemDetailViewModel.Price *= (100 + (decimal)this.CurrentBillDetailViewModel.VatPercentage) / 100;
                    }
                    else if (this.CurrentBillDetailViewModel.KindOfVat == KindOfVat.zzgl_MwSt)
                    {
                        billItemDetailViewModel.Price *= 100 / (100 + (decimal)this.CurrentBillDetailViewModel.VatPercentage);
                    }
                }

                this.CalculateSums();
            }
        }

        private void CalculateSums()
        {
            decimal sum = 0;

            foreach (BillItemDetailViewModel billItemDetailViewModel in this.BillItemDetailViewModels)
            {
                sum += billItemDetailViewModel.Sum;
            }

            if (this.currentBill.KindOfVat == KindOfVat.inkl_MwSt)
            {
                this.BruttoSum = sum;
                this.VatSum = this.BruttoSum / (decimal)(100 + this.CurrentBillDetailViewModel.VatPercentage) * (decimal)this.CurrentBillDetailViewModel.VatPercentage;
                this.NettoSum = this.BruttoSum - this.VatSum;
            }
            else if (this.currentBill.KindOfVat == KindOfVat.zzgl_MwSt)
            {
                this.NettoSum = sum;
                this.BruttoSum = sum * (100 + (decimal)this.CurrentBillDetailViewModel.VatPercentage) / 100;
                this.VatSum = this.BruttoSum - this.NettoSum;
            }
            else
            {
                this.NettoSum = sum;
                this.BruttoSum = sum;
                this.VatSum = 0;
            }

            this.RaisePropertyChanged(() => this.NettoSum);
            this.RaisePropertyChanged(() => this.VatSum);
            this.RaisePropertyChanged(() => this.BruttoSum);
        }

        private void ChangeVAT()
        {
            this.CanChangeVAT = !this.CanChangeVAT;
            this.RaisePropertyChanged(() => this.CanChangeVAT);

            if (this.CanChangeVAT == false)
            {
                this.CalculateSums();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= this.OnItemCollectionChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += this.OnItemCollectionChanged;
                }
            }
        }

        private void OnItemCollectionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CalculateSums();
        }

        private void InitBackgroundWorker()
        {
            this.createBillBackgroundWorker = new BackgroundWorker();
            this.createBillBackgroundWorker.DoWork += this.createBillBackgroundWorker_DoWork;
            this.createBillBackgroundWorker.RunWorkerCompleted += this.createBillBackgroundWorker_RunWorkerCompleted;
        }

        private void createBillBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.wordSerivce.CreateWordBill(this, (bool)e.Argument);
            e.Result = (bool)e.Argument;
        }

        private void createBillBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.dialogService.ShowMessage(Resources.Dialog_Title_Attention, e.Error.Message);
                this.wordSerivce.CloseDocument();
            }
            else
            {
                if ((bool)e.Result)
                {
                    this.dialogService.ShowMessage(Resources.Dialog_Title_Bill_Created, Resources.Dialog_Message_Bill_Created);
                }

                if ((bool)e.Result == false)
                {
                    if (this.wordSerivce.PrintDocument())
                    {
                        this.CurrentBillDetailViewModel.Printed = true;
                        this.repository.SaveOrUpdate(this.currentBill);
                        Messenger.Default.Send(new NotificationMessage<int>(this.currentBill.Id, Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM));
                    }

                    this.wordSerivce.CloseDocument();
                }
            }

            this.IsCreatingBill = false;
        }



        #region Messenger

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM)
            {
                this.AdjustSinglePricesToKindOfVat();
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Message_EnableStateForBillItemEditVM)
            {
                this.IsEditingEnabled = message.Content;
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_RemoveClientForBillItemEditVM)
            {
                if (message.Content == this.currentBill?.Client.Id)
                {
                    this.Clear();
                }
            }
        }

        #endregion



        #region Command Methods

        public List<ImageCommandViewModel> Commands { get; private set; }

        private void InitCommands()
        {
            this.Commands = new List<ImageCommandViewModel>
                            {
                                this.MoveItemUpCommand,
                                this.MoveItemDownCommand,
                                this.AddItemCommand,
                                this.DeleteItemCommand
                            };

            this.WordCommands = new List<ImageCommandViewModel>
                                {
                                    this.CreateDocumentCommand,
                                    this.PrintDocumentCommand
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
            BillItemDetailViewModel billItemDetailViewModel = new BillItemDetailViewModel(billItem, this.repository);

            this.currentBill.AddBillItem(billItem);
            this.BillItemDetailViewModels.Add(billItemDetailViewModel);
            Messenger.Default.Send(new NotificationMessage(Resources.Message_FocusBillItemsMessageForBillItemEditView));
        }

        public ImageCommandViewModel DeleteItemCommand
        {
            get
            {
                if (this._deleteItemCommand == null)
                {
                    this._deleteItemCommand = new ImageCommandViewModel(Resources.img_remove,
                                                                        Resources.Command_DisplayName_Delete,
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

        public List<ImageCommandViewModel> WordCommands { get; private set; }

        public ImageCommandViewModel CreateDocumentCommand
        {
            get
            {
                if (this._createDocumentCommand == null)
                {
                    this._createDocumentCommand = new ImageCommandViewModel(Resources.img_createWord,
                                                                            Resources.Command_DisplayName_Create,
                                                                            new RelayCommand(this.CreateDocument, this.CanCreateDocument));
                }

                return this._createDocumentCommand;
            }
        }

        public ImageCommandViewModel PrintDocumentCommand
        {
            get
            {
                if (this._printDocumentCommand == null)
                {
                    this._printDocumentCommand = new ImageCommandViewModel(Resources.img_print,
                                                                           Resources.Command_DisplayName_Print,
                                                                           new RelayCommand(this.PrintDocument, this.CanCreateDocument));
                }

                return this._printDocumentCommand;
            }
        }

        private bool CanCreateDocument()
        {
            if (this.IsEditingEnabled)
            {
                return false;
            }

            return true;
        }

        private void CreateDocument()
        {
            if (this.createBillBackgroundWorker.IsBusy)
            {
                return;
            }

            this.IsCreatingBill = true;
            this.createBillBackgroundWorker.RunWorkerAsync(true);
        }

        private void PrintDocument()
        {
            if (this.createBillBackgroundWorker.IsBusy)
            {
                return;
            }

            this.IsCreatingBill = true;
            this.createBillBackgroundWorker.RunWorkerAsync(false);
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

            foreach (ImageCommandViewModel imageCommandViewModel in this.WordCommands)
            {
                imageCommandViewModel.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}