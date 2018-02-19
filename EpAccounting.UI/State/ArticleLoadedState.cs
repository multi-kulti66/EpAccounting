// ///////////////////////////////////
// File: ArticleLoadedState.cs
// Last Change: 19.02.2018, 19:14
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using ViewModel;


    public class ArticleLoadedState : IArticleState
    {
        #region Fields

        private readonly ArticlesOptionViewModel _articlesOptionViewModel;

        #endregion



        #region Constructors

        public ArticleLoadedState(ArticlesOptionViewModel articlesOptionViewModel)
        {
            this._articlesOptionViewModel = articlesOptionViewModel;
        }

        #endregion



        #region IArticleState Members

        public bool CanSwitchToEditMode()
        {
            return true;
        }

        public void SwitchToEditMode()
        {
            this._articlesOptionViewModel.ChangeToEditMode();
        }

        public bool CanCommit()
        {
            return false;
        }

        public void Commit()
        {
            // do nothing
        }

        public bool CanCancel()
        {
            return false;
        }

        public void Cancel()
        {
            // do nothing
        }

        #endregion
    }
}