// ///////////////////////////////////
// File: OptionViewModel.cs
// Last Change: 17.08.2017  19:07
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Drawing;
    using System.IO;
    using EpAccounting.Business;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;



    public class OptionViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private RelayCommand _createDatabaseCommand;
        private RelayCommand _loadDatabaseCommand;

        #endregion



        #region Constructors / Destructor

        public OptionViewModel(string title, Bitmap image,
                               IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this.repository = repository;
            this.dialogService = dialogService;
        }

        #endregion



        #region Properties

        public string FilePath
        {
            get { return Settings.Default.DatabaseFilePath; }
            private set
            {
                if (this.SetProperty(Settings.Default.DatabaseFilePath, value, () => Settings.Default.DatabaseFilePath = value))
                {
                    Settings.Default.Save();
                }
            }
        }

        #endregion



        #region Commands

        public RelayCommand CreateDatabaseCommand
        {
            get
            {
                if (this._createDatabaseCommand == null)
                {
                    this._createDatabaseCommand = new RelayCommand(this.CreateDatabase);
                }

                return this._createDatabaseCommand;
            }
        }

        public RelayCommand LoadDatabaseCommand
        {
            get
            {
                if (this._loadDatabaseCommand == null)
                {
                    this._loadDatabaseCommand = new RelayCommand(this.LoadDatabase);
                }

                return this._loadDatabaseCommand;
            }
        }

        private async void CreateDatabase()
        {
            string databaseFolderPath = this.dialogService.ShowFolderDialog();

            if (string.IsNullOrEmpty(databaseFolderPath))
            {
                return;
            }

            try
            {
                string databaseFilePath = Path.Combine(databaseFolderPath, Resources.Database_NameWithExtension);

                if (File.Exists(databaseFilePath))
                {
                    bool shouldOverwrite = await this.dialogService.ShowDialogYesNo(Resources.Dialog_Title_DatabaseAlreadyExists,
                                                                                    string.Format(Resources.Dialog_Question_ShouldOverwriteDatabase, databaseFilePath));

                    if (!shouldOverwrite)
                    {
                        return;
                    }
                }

                this.repository.CreateDatabase(databaseFilePath);
                this.UpdatePathAndConnectionState(databaseFilePath);
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Exception_Message_CouldNotCreateDatabase, e.Message);
            }
        }

        private void LoadDatabase()
        {
            string databaseFilePath = this.dialogService.ShowDatabaseFileDialog();

            if (string.IsNullOrEmpty(databaseFilePath))
            {
                return;
            }

            try
            {
                this.repository.LoadDatabase(databaseFilePath);
                this.UpdatePathAndConnectionState(databaseFilePath);
            }
            catch (Exception e)
            {
                this.dialogService.ShowMessage(Resources.Exception_Message_CouldNotLoadDatabase, e.Message);
            }
        }

        private void UpdatePathAndConnectionState(string databaseFilePath)
        {
            this.FilePath = databaseFilePath;
            Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateConnectionStateForMainVM));
        }

        #endregion
    }
}