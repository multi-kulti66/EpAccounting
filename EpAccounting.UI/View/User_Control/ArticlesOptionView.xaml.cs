// ///////////////////////////////////
// File: ArticlesOptionView.xaml.cs
// Last Change: 04.11.2017  08:40
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.User_Control
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Messaging;



    /// <summary>
    ///     Interaction logic for ArticlesOptionView.xaml
    /// </summary>
    public partial class ArticlesOptionView : UserControl
    {
        #region Fields

        private bool unloadedRow;

        #endregion



        #region Constructors / Destructor

        public ArticlesOptionView()
        {
            this.InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Properties.Resources.Message_FocusArticleForArticlesOptionView)
            {
                this.SetFocusOnCell();
            }
        }

        private void SetFocusOnCell()
        {
            int lastItemIndex = this.ArticleDataGrid.Items.Count - 1;
            int columnIndex = 0;

            this.ArticleDataGrid.Focus();
            this.ArticleDataGrid.SelectedItem = this.ArticleDataGrid.Items[lastItemIndex];
            this.ArticleDataGrid.CurrentCell = new DataGridCellInfo(this.ArticleDataGrid.Items[lastItemIndex],
                                                                    this.ArticleDataGrid.Columns[columnIndex]);
            this.ArticleDataGrid.BeginEdit();
        }

        private void ArticleDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.unloadedRow && this.ArticleDataGrid.SelectedIndex >= 0)
            {
                this.unloadedRow = false;
                DataGridRow selectedRow = (DataGridRow)this.ArticleDataGrid.ItemContainerGenerator.ContainerFromIndex(this.ArticleDataGrid.SelectedIndex);
                selectedRow?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void ArticleDataGrid_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            this.unloadedRow = true;
        }
    }
}