// ///////////////////////////////////
// File: IArticleState.cs
// Last Change: 16.09.2017  10:51
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    public interface IArticleState
    {
        bool CanSwitchToEditMode();

        void SwitchToEditMode();

        bool CanCommit();

        void Commit();

        bool CanCancel();

        void Cancel();
    }
}