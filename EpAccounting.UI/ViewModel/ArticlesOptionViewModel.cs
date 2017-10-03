// ///////////////////////////////////
// File: ArticlesOptionViewModel.cs
// Last Change: 17.09.2017  16:34
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Markup;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;



    public class ArticlesOptionViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private IArticleState _currentState;
        private ArticleLoadedState _articleLoadedState;
        private ArticleEditState _articleEditState;

        private ArticleViewModel _selectedArticleViewModel;
        private ImageCommandViewModel _addItemCommand;
        private ImageCommandViewModel _deleteItemCommand;

        private ImageCommandViewModel _switchToEditModeCommand;
        private ImageCommandViewModel _saveOrUpdateCommand;
        private ImageCommandViewModel _cancelCommand;

        #endregion



        #region Constructors / Destructor

        public ArticlesOptionViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService)
            : base(title, image)
        {
            this.repository = repository;
            this.dialogService = dialogService;
            this.ArticleViewModels = new ObservableCollection<ArticleViewModel>();

            this.InitCommands();
            this.InitStates();
            this.InitStateCommands();
            this.TryLoadingArticlesOnStartup();
        }

        #endregion



        #region Properties

        public IArticleState CurrentState
        {
            get { return this._currentState; }
            private set
            {
                this.SetProperty(ref this._currentState, value);
                this.UpdateCommands();
                this.UpdateStateCommands();
                this.SendWorkspaceEnabledMessage();
            }
        }

        public ArticleViewModel SelectedArticleViewModel
        {
            get { return this._selectedArticleViewModel; }
            set
            {
                this.SetProperty(ref this._selectedArticleViewModel, value);
                this.UpdateCommands();
            }
        }

        public ObservableCollection<ArticleViewModel> ArticleViewModels { get; }

        #endregion



        public virtual void RestoreArticles()
        {
            this.ArticleViewModels.Clear();
            this.LoadArticles();
        }

        public virtual bool SaveChanges()
        {
            if (this.AreArticleNumbersUnique() == false)
            {
                this.dialogService.ShowMessage(Resources.Dialog_Title_Attention, Resources.Dialog_Message_MultipleEqualArticleNumbers);
                return false;
            }

            List<ArticleViewModel> orderedCollection = this.ArticleViewModels.OrderBy(x => x.ArticleNumber).ToList();
            this.ArticleViewModels.Clear();

            foreach (ArticleViewModel articleViewModel in orderedCollection)
            {
                this.ArticleViewModels.Add(articleViewModel);
                articleViewModel.Save();
            }

            this.RemoveDeletedArticlesFromDatabase();

            return true;
        }

        private bool AreArticleNumbersUnique()
        {
            foreach (ArticleViewModel articleViewModel in this.ArticleViewModels)
            {
                if (this.ArticleViewModels.Count(x => x.ArticleNumber == articleViewModel.ArticleNumber) > 1)
                {
                    return false;
                }
            }

            return true;
        }

        private void RemoveDeletedArticlesFromDatabase()
        {
            Action<Article> action = delegate(Article article)
            {
                if (this.ArticleViewModels.Count(x => x.Id == article.Id) == 0)
                {
                    this.repository.Delete(article);
                }
            };

            this.IterateArticlesAndDoAction(action);
        }

        private void TryLoadingArticlesOnStartup()
        {
            try
            {
                this.LoadArticles();
            }
            catch
            {
                // do nothing
            }
        }

        private void LoadArticles()
        {
            this.IterateArticlesAndDoAction(article => this.ArticleViewModels
                                                           .InsertOrderedBy(new ArticleViewModel(article, this.repository),
                                                                            x => x.ArticleNumber));
        }

        private void IterateArticlesAndDoAction(Action<Article> action)
        {
            int numberOfArticles = this.repository.GetQuantity<Article>();
            int articlePages;

            if (numberOfArticles % Settings.Default.PageSize == 0)
            {
                articlePages = numberOfArticles / Settings.Default.PageSize;
            }
            else
            {
                articlePages = numberOfArticles / Settings.Default.PageSize + 1;
            }

            for (int i = 1; i <= articlePages; i++)
            {
                foreach (Article article in this.repository.GetAll<Article>(i))
                {
                    action(article);
                }
            }
        }



        #region Commands

        public List<ImageCommandViewModel> Commands { get; private set; }

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

        public ImageCommandViewModel DeleteItemCommand
        {
            get
            {
                if (this._deleteItemCommand == null)
                {
                    this._deleteItemCommand = new ImageCommandViewModel(Resources.img_delete,
                                                                        Resources.Command_DisplayName_Delete,
                                                                        new RelayCommand(this.DeleteItem, this.CanDeleteItem));
                }

                return this._deleteItemCommand;
            }
        }

        private bool CanAddItem()
        {
            if (this.CurrentState == this.GetEditState())
            {
                return true;
            }

            return false;
        }

        private void AddItem()
        {
            ArticleViewModel articleViewModel = new ArticleViewModel(new Article(), this.repository);

            this.ArticleViewModels.Add(articleViewModel);
            Messenger.Default.Send(new NotificationMessage(Resources.Message_FocusArticleForArticlesOptionView));
        }

        private bool CanDeleteItem()
        {
            if (this.CurrentState == this.GetEditState() && this.SelectedArticleViewModel != null)
            {
                return true;
            }

            return false;
        }

        private void DeleteItem()
        {
            this.ArticleViewModels.Remove(this.ArticleViewModels.First(x => x.Id == this.SelectedArticleViewModel.Id));
            this.SelectedArticleViewModel = null;
        }

        private void InitCommands()
        {
            this.Commands = new List<ImageCommandViewModel>
                            {
                                this.AddItemCommand,
                                this.DeleteItemCommand
                            };
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel imageCommandViewModel in this.Commands)
            {
                imageCommandViewModel.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        private void SendWorkspaceEnabledMessage()
        {
            if (this.CurrentState == this.GetLoadedState())
            {
                Messenger.Default.Send(new NotificationMessage<bool>(true,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false,
                                                                     Resources.Message_WorkspaceEnableStateForMainVM));
            }
            
        }

        #endregion



        #region States

        public IArticleState GetLoadedState()
        {
            return this._articleLoadedState;
        }

        public IArticleState GetEditState()
        {
            return this._articleEditState;
        }

        public virtual void ChangeToEditMode()
        {
            this.CurrentState = this.GetEditState();
        }

        public virtual void ChangeToLoadedMode()
        {
            this.CurrentState = this.GetLoadedState();
        }

        private void InitStates()
        {
            this._articleLoadedState = new ArticleLoadedState(this);
            this._articleEditState = new ArticleEditState(this);

            this._currentState = this.GetLoadedState();
        }

        #endregion



        #region State Commands

        public List<ImageCommandViewModel> StateCommands { get; private set; }

        public ImageCommandViewModel SwitchToEditModeCommand
        {
            get
            {
                if (this._switchToEditModeCommand == null)
                {
                    this._switchToEditModeCommand = new ImageCommandViewModel(Resources.img_edit,
                                                                              Resources.Command_DisplayName_Edit,
                                                                              new RelayCommand(this.SwitchToEditMode, this.CanSwitchToEditMode));
                }

                return this._switchToEditModeCommand;
            }
        }

        public ImageCommandViewModel SaveOrUpdateCommand
        {
            get
            {
                if (this._saveOrUpdateCommand == null)
                {
                    this._saveOrUpdateCommand = new ImageCommandViewModel(Resources.img_saveOrUpdate,
                                                                          Resources.Command_DisplayName_SaveOrUpdate,
                                                                          new RelayCommand(this.Commit, this.CanCommit));
                }

                return this._saveOrUpdateCommand;
            }
        }

        public ImageCommandViewModel CancelCommand
        {
            get
            {
                if (this._cancelCommand == null)
                {
                    this._cancelCommand = new ImageCommandViewModel(Resources.img_cancel,
                                                                    Resources.Command_DisplayName_Cancel,
                                                                    new RelayCommand(this.Cancel, this.CanCancel));
                }

                return this._cancelCommand;
            }
        }

        private void InitStateCommands()
        {
            this.StateCommands = new List<ImageCommandViewModel>
                                 {
                                     this.SwitchToEditModeCommand,
                                     this.SaveOrUpdateCommand,
                                     this.CancelCommand
                                 };
        }

        private bool CanSwitchToEditMode()
        {
            return this.repository.IsConnected && this.CurrentState.CanSwitchToEditMode();
        }

        private void SwitchToEditMode()
        {
            this.CurrentState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            return this.repository.IsConnected && this._currentState.CanCommit();
        }

        private void Commit()
        {
            this.CurrentState.Commit();
        }

        private bool CanCancel()
        {
            return this.repository.IsConnected && this.CurrentState.CanCancel();
        }

        private void Cancel()
        {
            this.CurrentState.Cancel();
        }

        private void UpdateStateCommands()
        {
            foreach (ImageCommandViewModel imageCommandViewModel in this.StateCommands)
            {
                imageCommandViewModel.RelayCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}