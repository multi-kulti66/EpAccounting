// ///////////////////////////////////
// File: IBillState.cs
// Last Change: 23.04.2017  19:49
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;



    public interface IBillState
    {
        bool CanSwitchToSearchMode();

        void SwitchToSearchMode();

        bool CanSwitchToEditMode();

        void SwitchToEditMode();

        bool CanCommit();

        Task Commit();

        bool CanCancel();

        void Cancel();

        bool CanDelete();

        Task Delete();
    }
}