// ///////////////////////////////////
// File: ArticleLoadedState.cs
// Last Change: 22.10.2017  16:05
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using EpAccounting.UI.ViewModel;



    public class ArticleLoadedState : IArticleState
    {
        #region Fields

        private readonly ArticlesOptionViewModel articlesOptionViewModel;

        #endregion



        #region Constructors / Destructor

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