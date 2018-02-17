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

        private readonly ArticlesOptionViewModel articlesOptionViewModel;

        #endregion



        #region Constructors

        public ArticleEditState(ArticlesOptionViewModel articlesOptionViewModel)
        {
            this.articlesOptionViewModel = articlesOptionViewModel;
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
            if (this.articlesOptionViewModel.SaveChanges())
            {
                this.articlesOptionViewModel.ChangeToLoadedMode();
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this.articlesOptionViewModel.RestoreArticles();
            this.articlesOptionViewModel.ChangeToLoadedMode();
        }

        #endregion
    }
}