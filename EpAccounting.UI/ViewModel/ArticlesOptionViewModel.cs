// ///////////////////////////////////
// File: ArticlesOptionViewModel.cs
// Last Change: 19.02.2018, 19:40
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Markup;
    using Model;
    using Properties;
    using Service;
    using State;


    public class ArticlesOptionViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

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



        #region Constructors

        public ArticlesOptionViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService)
            : base(title, image)
        {
            this._repository = repository;
            this._dialogService = dialogService;
            this.ArticleViewModels = new ObservableCollection<ArticleViewModel>();

            this.InitCommands();
            this.InitStates();
            this.InitStateCommands();
            this.TryLoadingArticlesOnStartup();
        }

        #endregion



        #region Properties, Indexers

        public bool IsEditable
        {
            get { return this.CurrentState == this._articleEditState; }
        }

        public IArticleState CurrentState
        {
            get { return this._currentState; }
            private set
            {
                if (this.SetProperty(ref this._currentState, value))
                {
                    this.RaisePropertyChanged(() => this.IsEditable);
                    this.UpdateCommands();
                    this.UpdateStateCommands();
                    this.SendWorkspaceEnabledMessage();
                }
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
                this._dialogService.ShowMessage(Resources.Dialog_Title_Attention, Resources.Dialog_Message_MultipleEqualArticleNumbers);
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
            void Delete(Article article)
            {
                if (this.ArticleViewModels.Count(x => x.Id == article.Id) != 0)
                {
                    return;
                }

                try
                {
                    this._repository.Delete(article);
                }
                catch (Exception e)
                {
                    this._dialogService.ShowExceptionMessage(e, $"Could not delete article '{article.Description}' from database!");
                }
            }

            this.IterateArticlesAndDoAction(Delete);
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
                                                           .InsertOrderedBy(new ArticleViewModel(article, this._repository, this._dialogService),
                                                                            x => x.ArticleNumber));
        }

        private void IterateArticlesAndDoAction(Action<Article> action)
        {
            int numberOfArticles = this._repository.GetQuantity<Article>();
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
                foreach (Article article in this._repository.GetAll<Article>(i))
                {
                    action(article);
                }
            }
        }

        private bool CanAddItem()
        {
            return this.CurrentState == this.GetEditState();
        }

        private void AddItem()
        {
            ArticleViewModel articleViewModel = new ArticleViewModel(new Article(), this._repository, this._dialogService);

            this.ArticleViewModels.Add(articleViewModel);
            Messenger.Default.Send(new NotificationMessage(Resources.Message_FocusArticleForArticlesOptionView));
        }

        private bool CanDeleteItem()
        {
            return this.CurrentState == this.GetEditState() && this.SelectedArticleViewModel != null;
        }

        private void DeleteItem()
        {
            this.ArticleViewModels.Remove(this.ArticleViewModels.First(x => x.ArticleNumber == this.SelectedArticleViewModel.ArticleNumber &&
                                                                            x.Description == this.SelectedArticleViewModel.Description &&
                                                                            Math.Abs(x.Amount - this.SelectedArticleViewModel.Amount) < 0.01 &&
                                                                            x.Price == this.SelectedArticleViewModel.Price));
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
            return this._repository.IsConnected && this.CurrentState.CanSwitchToEditMode();
        }

        private void SwitchToEditMode()
        {
            this.CurrentState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            return this._repository.IsConnected && this._currentState.CanCommit();
        }

        private void Commit()
        {
            this.CurrentState.Commit();
        }

        private bool CanCancel()
        {
            return this._repository.IsConnected && this.CurrentState.CanCancel();
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
    }
}