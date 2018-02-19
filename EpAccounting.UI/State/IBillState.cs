// ///////////////////////////////////
// File: IBillState.cs
// Last Change: 19.02.2018, 19:14
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