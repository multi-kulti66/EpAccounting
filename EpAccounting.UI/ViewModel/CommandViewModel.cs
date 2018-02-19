// ///////////////////////////////////
// File: CommandViewModel.cs
// Last Change: 19.02.2018, 20:05
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using GalaSoft.MvvmLight.Command;


    public class CommandViewModel : BindableViewModelBase
    {
        #region Constructors

        public CommandViewModel(string displayName, RelayCommand relayCommand)
        {
            this.DisplayName = displayName;
            this.RelayCommand = relayCommand;
        }

        #endregion



        #region Properties, Indexers

        public string DisplayName { get; private set; }

        public RelayCommand RelayCommand { get; private set; }

        #endregion
    }
}