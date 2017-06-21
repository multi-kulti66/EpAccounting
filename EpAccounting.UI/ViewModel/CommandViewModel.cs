// ///////////////////////////////////
// File: CommandViewModel.cs
// Last Change: 14.03.2017  17:00
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using GalaSoft.MvvmLight.Command;



    public class CommandViewModel : BindableViewModelBase
    {
        #region Constructors / Destructor

        public CommandViewModel(string displayName, string commandMessage, RelayCommand relayCommand)
        {
            this.DisplayName = displayName;
            this.CommandMessage = commandMessage;
            this.RelayCommand = relayCommand;
        }

        #endregion



        #region Properties

        public string DisplayName { get; private set; }

        public string CommandMessage { get; private set; }

        public RelayCommand RelayCommand { get; private set; }

        #endregion
    }
}