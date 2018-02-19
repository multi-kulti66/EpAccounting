// ///////////////////////////////////
// File: ArticleEditState.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using ViewModel;


    public class ArticleEditState : IArticleState
    {
        #region Fields

        private readonly ArticlesOptionViewModel _articlesOptionViewModel;

        #endregion



        #region Constructors

        public ArticleEditState(ArticlesOptionViewModel articlesOptionViewModel)
        {
            this._articlesOptionViewModel = articlesOptionViewModel;
        }

        #endregion



        #region IArticleState Members

        public bool CanSwitchToEditMode()
        {
            return false;
        }

        public void SwitchToEditMode()
        {
            // do nothing
        }

        public bool CanCommit()
        {
            return true;
        }

        public void Commit()
        {
            if (this._articlesOptionViewModel.SaveChanges())
            {
                this._articlesOptionViewModel.ChangeToLoadedMode();
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this._articlesOptionViewModel.RestoreArticles();
            this._articlesOptionViewModel.ChangeToLoadedMode();
        }

        #endregion
    }
}