// ///////////////////////////////////
// File: BillItemEditView.xaml.cs
// Last Change: 23.06.2017  22:17
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.UserControl
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
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



        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }

            return null;
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }

                T childOfChild = FindVisualChild<T>(child);

                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }

        private void ExecuteNotificationMessage(NotificationMessage obj)
        {
            if (obj.Notification == Properties.Resources.Messenger_Message_FocusBillItems)
            {
                int index = this.DataGrid_BillItems.Items.Count - 1;

                this.DataGrid_BillItems.SelectedItems.Clear();
                object item = this.DataGrid_BillItems.Items[index];
                this.DataGrid_BillItems.SelectedItem = item;

                DataGridRow row = this.DataGrid_BillItems.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                if (row == null)
                {
                    /* bring the data item (Product object) into view
                     * in case it has been virtualized away */
                    this.DataGrid_BillItems.ScrollIntoView(item);
                    row = this.DataGrid_BillItems.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                }

                if (row != null)
                {
                    DataGridCell cell = GetCell(this.DataGrid_BillItems, row, 1);
                    if (cell != null)
                    {
                        cell.Focus();

                        MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                                                    {
                                                        RoutedEvent = MouseLeftButtonDownEvent,
                                                        Source = cell
                                                    };
                        cell.RaiseEvent(args);
                    }
                }
            }
        }
    }
}