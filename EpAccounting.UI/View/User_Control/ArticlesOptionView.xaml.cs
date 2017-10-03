// ///////////////////////////////////
// File: ArticlesOptionView.xaml.cs
// Last Change: 10.09.2017  15:28
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.User_Control
{
    using System.Windows.Controls;
    using GalaSoft.MvvmLight.Messaging;



    /// <summary>
    ///     Interaction logic for ArticlesOptionView.xaml
    /// </summary>
    public partial class ArticlesOptionView : UserControl
    {
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
    }
}