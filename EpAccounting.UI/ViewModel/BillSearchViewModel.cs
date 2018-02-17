// ///////////////////////////////////
// File: BillSearchViewModel.cs
// Last Change: 17.02.2018, 21:48
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using NHibernate.Criterion;
    using Properties;


    public class BillSearchViewModel : BillWorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;

        private int _numberOfAllPages;
        private int _currentPage;
        private ObservableCollection<BillDetailViewModel> _foundBills;
        private BillDetailViewModel _selectedBillDetailViewModel;

        private RelayCommand _loadSelectedBillCommand;
        private ImageCommandViewModel _loadFirstPageCommand;
        private ImageCommandViewModel _loadLastPageCommand;
        private ImageCommandViewModel _loadNextPageCommand;
        private ImageCommandViewModel _loadPreviousPageCommand;

        #endregion



        #region Constructors

        public BillSearchViewModel(IRepository repository)
        {
            this.repository = repository;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public int NumberOfAllPages
        {
            get { return this._numberOfAllPages; }
            private set { this.SetProperty(ref this._numberOfAllPages, value); }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            private set { this.SetProperty(ref this._currentPage, value); }
        }

        public ObservableCollection<BillDetailViewModel> FoundBills
        {
            get
            {
                if (this._foundBills == null)
                {
                    this._foundBills = new ObservableCollection<BillDetailViewModel>();
                }

                return this._foundBills;
            }
        }

        public BillDetailViewModel SelectedBillDetailViewModel
        {
            get { return this._selectedBillDetailViewModel; }
            set { this.SetProperty(ref this._selectedBillDetailViewModel, value); }
        }

        public RelayCommand LoadSelectedBillCommand
        {
            get
            {
                if (this._loadSelectedBillCommand == null)
                {
                    this._loadSelectedBillCommand = new RelayCommand(this.SendLoadSelectedBillMessage, this.CanLoadSelectedBill);
                }

                return this._loadSelectedBillCommand;
            }
        }

        public ImageCommandViewModel LoadFirstPageCommand
        {
            get
            {
                if (this._loadFirstPageCommand == null)
                {
                    this._loadFirstPageCommand = new ImageCommandViewModel(Resources.img_arrow_first,
                                                                           Resources.Command_DisplayName_First_Page,
                                                                           new RelayCommand(this.LoadFirstPage, this.CanLoadFirstPage));
                }

                return this._loadFirstPageCommand;
            }
        }

        public ImageCommandViewModel LoadLastPageCommand
        {
            get
            {
                if (this._loadLastPageCommand == null)
                {
                    this._loadLastPageCommand = new ImageCommandViewModel(Resources.img_arrow_last,
                                                                          Resources.Command_DisplayName_Last_Page,
                                                                          new RelayCommand(this.LoadLastPage, this.CanLoadLastPage));
                }

                return this._loadLastPageCommand;
            }
        }

        public ImageCommandViewModel LoadNextPageCommand
        {
            get
            {
                if (this._loadNextPageCommand == null)
                {
                    this._loadNextPageCommand = new ImageCommandViewModel(Resources.img_arrow_right,
                                                                          Resources.Command_DisplayName_Next_Page,
                                                                          new RelayCommand(this.LoadNextPage, this.CanLoadNextPage));
                }

                return this._loadNextPageCommand;
            }
        }

        public ImageCommandViewModel LoadPreviousPageCommand
        {
            get
            {
                if (this._loadPreviousPageCommand == null)
                {
                    this._loadPreviousPageCommand = new ImageCommandViewModel(Resources.img_arrow_left,
                                                                              Resources.Command_DisplayName_Previous_Page,
                                                                              new RelayCommand(this.LoadPreviousPage, this.CanLoadPreviousPage));
                }

                return this._loadPreviousPageCommand;
            }
        }

        private Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> LastCriterion { get; set; }

        #endregion



        private void LoadBillsFromClient(int clientId)
        {
            Conjunction billConjunction = Restrictions.Conjunction();
            Expression<Func<Bill, Client>> expression = x => x.Client;
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(client => client.Id == clientId));

            this.LoadSearchedBills(new Tuple<ICriterion,
                                       Expression<Func<Bill, Client>>,
                                       ICriterion>(billConjunction, expression, clientConjunction));
        }

        private void LoadSearchedBills(Tuple<ICriterion,
                                           Expression<Func<Bill, Client>>,
                                           ICriterion> tupleCriterion, int page = 1)
        {
            // ReSharper disable once PossibleLossOfFraction
            int numberOfBills;

            if (tupleCriterion.Item2 == null || tupleCriterion.Item3 == null)
            {
                numberOfBills = this.repository.GetQuantityByCriteria<Bill>(tupleCriterion.Item1);
            }
            else
            {
                numberOfBills = this.repository.GetQuantityByCriteria(tupleCriterion.Item1, tupleCriterion.Item2, tupleCriterion.Item3);
            }

            this.NumberOfAllPages = (numberOfBills - 1) / Settings.Default.PageSize + 1;
            this.CurrentPage = this.CurrentPage > this.NumberOfAllPages ? this.NumberOfAllPages : page;
            this.ReloadCommands();

            this.FoundBills.Clear();

            if (tupleCriterion.Item2 == null || tupleCriterion.Item3 == null)
            {
                foreach (Bill bill in this.repository.GetByCriteria<Bill>(tupleCriterion.Item1, this.CurrentPage).ToList())
                {
                    this.FoundBills.Add(new BillDetailViewModel(bill, this.repository));
                }
            }
            else
            {
                foreach (Bill bill in this.repository.GetByCriteria(tupleCriterion.Item1, tupleCriterion.Item2, tupleCriterion.Item3, this.CurrentPage).ToList())
                {
                    this.FoundBills.Add(new BillDetailViewModel(bill, this.repository));
                }
            }
        }

        private void UpdateBillViaBillId(int id)
        {
            for (int i = 0; i < this.FoundBills.Count; i++)
            {
                if (this.FoundBills[i].Id == id)
                {
                    Bill bill = this.repository.GetById<Bill>(id);
                    this.FoundBills[i] = new BillDetailViewModel(bill, this.repository);
                }
            }
        }

        private void UpdateBillViaClientId(int id)
        {
            for (int i = 0; i < this.FoundBills.Count; i++)
            {
                if (this.FoundBills[i].ClientId == id)
                {
                    Bill bill = this.repository.GetById<Bill>(this.FoundBills[i].Id);
                    this.FoundBills[i] = new BillDetailViewModel(bill, this.repository);
                }
            }
        }

        private void ReloadBill()
        {
            this.LoadSearchedBills(this.LastCriterion, this.CurrentPage);
        }

        private void RemoveBillsViaClientId(int id)
        {
            for (int i = this.FoundBills.Count - 1; i >= 0; i--)
            {
                if (this.FoundBills[i].ClientId == id)
                {
                    this.FoundBills.RemoveAt(i);
                }
            }
        }

        private void ReloadCommands()
        {
            this.LoadPreviousPageCommand.RelayCommand.RaiseCanExecuteChanged();
            this.LoadNextPageCommand.RelayCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteNotificationMessage(NotificationMessage<Tuple<ICriterion,
                                                    Expression<Func<Bill, Client>>,
                                                    ICriterion>> message)
        {
            if (message.Notification == Resources.Message_BillSearchCriteriaForBillSearchVM)
            {
                this.LastCriterion = message.Content;
                this.LoadSearchedBills(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_ReloadBillForBillSearchVM)
            {
                this.ReloadBill();
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_UpdateBillValuesMessageForBillSearchVM)
            {
                this.UpdateBillViaBillId(message.Content);
            }
            else if (message.Notification == Resources.Message_UpdateClientValuesForBillSearchVM)
            {
                this.UpdateBillViaClientId(message.Content);
            }
            else if (message.Notification == Resources.Message_LoadBillsFromClientForBillSearchVM)
            {
                this.LoadBillsFromClient(message.Content);
            }
            else if (message.Notification == Resources.Message_RemoveClientForBillSearchVM)
            {
                this.RemoveBillsViaClientId(message.Content);
            }
        }

        private bool CanLoadSelectedBill()
        {
            if (this.SelectedBillDetailViewModel == null)
            {
                return false;
            }

            return true;
        }

        private void SendLoadSelectedBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.SelectedBillDetailViewModel.Id, Resources.Message_LoadSelectedBillForBillEditVM));
        }

        private bool CanLoadFirstPage()
        {
            return this.CurrentPage > 1;
        }

        private void LoadFirstPage()
        {
            this.CurrentPage = 1;
            this.LoadSearchedBills(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadLastPage()
        {
            return this.CurrentPage < this.NumberOfAllPages;
        }

        private void LoadLastPage()
        {
            this.CurrentPage = this.NumberOfAllPages;
            this.LoadSearchedBills(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadNextPage()
        {
            if (this.CurrentPage < this.NumberOfAllPages)
            {
                return true;
            }

            return false;
        }

        private void LoadNextPage()
        {
            this.CurrentPage++;
            this.LoadSearchedBills(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadPreviousPage()
        {
            if (this.CurrentPage > 1)
            {
                return true;
            }

            return false;
        }

        private void LoadPreviousPage()
        {
            this.CurrentPage--;
            this.LoadSearchedBills(this.LastCriterion, this.CurrentPage);
        }
    }
}