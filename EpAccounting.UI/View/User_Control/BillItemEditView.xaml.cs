// ///////////////////////////////////
// File: BillItemEditView.xaml.cs
// Last Change: 04.11.2017  08:42
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.User_Control
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Messaging;



    /// <summary>
    ///     Interaction logic for BillItemEditView.xaml
    /// </summary>
    public partial class BillItemEditView : UserControl
    {
        #region Fields

        private bool unloadedRow;

        #endregion



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
            int lastItemIndex = this.BillDataGrid.Items.Count - 1;
            int columnIndex = 1;

            this.BillDataGrid.Focus();
            this.BillDataGrid.SelectedItem = this.BillDataGrid.Items[lastItemIndex];
            this.BillDataGrid.CurrentCell = new DataGridCellInfo(this.BillDataGrid.Items[lastItemIndex], this.BillDataGrid.Columns[columnIndex]);
            this.BillDataGrid.BeginEdit();
        }

        private void DataGrid_BillItems_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            this.unloadedRow = true;
        }

        private void DataGrid_BillItems_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.unloadedRow && this.BillDataGrid.SelectedIndex >= 0)
            {
                this.unloadedRow = false;
                DataGridRow selectedRow = (DataGridRow)this.BillDataGrid.ItemContainerGenerator.ContainerFromIndex(this.BillDataGrid.SelectedIndex);
                selectedRow?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}