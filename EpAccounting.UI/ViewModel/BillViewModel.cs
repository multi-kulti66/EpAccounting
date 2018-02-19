// ///////////////////////////////////
// File: BillViewModel.cs
// Last Change: 19.02.2018, 19:54
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Drawing;
    using Business;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using Properties;
    using Service;


    public class BillViewModel : WorkspaceViewModel, IDisposable
    {
        #region Fields

        private BillWorkspaceViewModel _billWorkspaceViewModel;

        #endregion



        #region Constructors

        public BillViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this.BillEditViewModel = new BillEditViewModel(repository, dialogService);
            this.BillItemEditViewModel = new BillItemEditViewModel(repository, dialogService, new WordService());
            this.BillSearchViewModel = new BillSearchViewModel(repository, dialogService);
            this._billWorkspaceViewModel = this.BillSearchViewModel;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<Bill>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public BillEditViewModel BillEditViewModel { get; }

        public BillWorkspaceViewModel BillWorkspaceViewModel
        {
            get { return this._billWorkspaceViewModel; }
            set { this.SetProperty(ref this._billWorkspaceViewModel, value); }
        }

        private BillItemEditViewModel BillItemEditViewModel { get; }

        private BillSearchViewModel BillSearchViewModel { get; }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            this.BillEditViewModel?.Dispose();
            this.BillItemEditViewModel?.Dispose();
            this.BillSearchViewModel?.Dispose();
            Messenger.Default.Unregister(this);
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_LoadBillSearchViewModelMessageForBillVM)
            {
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
            else if (message.Notification == Resources.Message_ResetBillItemEditVMAndChangeToSearchWorkspaceForBillVM)
            {
                this.BillItemEditViewModel.Clear();
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<Bill> message)
        {
            if (message.Notification == Resources.Message_LoadBillItemEditViewModelForBillVM)
            {
                this.BillWorkspaceViewModel = this.BillItemEditViewModel;
                this.BillItemEditViewModel.LoadBill(message.Content);
            }
        }
    }
}