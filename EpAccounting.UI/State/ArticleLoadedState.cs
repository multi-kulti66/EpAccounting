// ///////////////////////////////////
// File: ArticleLoadedState.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using ViewModel;


    public class ArticleLoadedState : IArticleState
    {
        #region Fields

        private readonly ArticlesOptionViewModel articlesOptionViewModel;

        #endregion



        #region Constructors

        public ArticleLoadedState(ArticlesOptionViewModel articlesOptionViewModel)
        {
            this.articlesOptionViewModel = articlesOptionViewModel;
        }

        #endregion



        #region IArticleState Members

        public bool CanSwitchToEditMode()
        {
            return true;
        }

        public void SwitchToEditMode()
        {
            this.articlesOptionViewModel.ChangeToEditMode();
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