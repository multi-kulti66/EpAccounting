// ///////////////////////////////////
// File: IClientState.cs
// Last Change: 03.03.2017  20:58
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;



    public interface IClientState
    {
        bool CanSwitchToSearchMode();

        void SwitchToSearchMode();

        bool CanSwitchToAddMode();

        void SwitchToAddMode();

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