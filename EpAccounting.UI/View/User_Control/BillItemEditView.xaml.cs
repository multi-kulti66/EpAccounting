﻿// ///////////////////////////////////
// File: BillItemEditView.xaml.cs
// Last Change: 07.09.2017  21:00
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.User_Control
{
    using System.Windows.Controls;
    using GalaSoft.MvvmLight.Messaging;



    /// <summary>
    ///     Interaction logic for BillItemEditView.xaml
    /// </summary>
    public partial class BillItemEditView : UserControl
    {
        #region Constructors / Destructor

        public BillItemEditView()
        {
            this.InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Properties.Resources.Message_FocusBillItemsMessageForBillItemEditView)
            {
                this.SetFocusOnCell();
            }
        }

        private void SetFocusOnCell()
        {
            int lastItemIndex = this.dataGrid_BillItems.Items.Count - 1;
            int columnIndex = 1;

            this.dataGrid_BillItems.Focus();
            this.dataGrid_BillItems.SelectedItem = this.dataGrid_BillItems.Items[lastItemIndex];
            this.dataGrid_BillItems.CurrentCell = new DataGridCellInfo(this.dataGrid_BillItems.Items[lastItemIndex], this.dataGrid_BillItems.Columns[columnIndex]);
            this.dataGrid_BillItems.BeginEdit();
        }
    }
}