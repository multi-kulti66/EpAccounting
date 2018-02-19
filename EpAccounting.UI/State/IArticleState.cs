// ///////////////////////////////////
// File: IArticleState.cs
// Last Change: 19.02.2018, 19:14
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