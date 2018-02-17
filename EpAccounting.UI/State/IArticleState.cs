// ///////////////////////////////////
// File: IArticleState.cs
// Last Change: 17.02.2018, 14:28
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