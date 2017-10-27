// ///////////////////////////////////
// File: IArticleState.cs
// Last Change: 22.10.2017  16:05
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