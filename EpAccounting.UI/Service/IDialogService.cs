// ///////////////////////////////////
// File: IDialogService.cs
// Last Change: 17.02.2018, 19:11
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.Service
{
    using System;
    using System.Drawing.Printing;
    using System.Threading.Tasks;


    public interface IDialogService
    {
        Task ShowMessage(string title, string message);

        Task ShowExceptionMessage(Exception e, string title = "An error occured!");

        Task<bool> ShowDialogYesNo(string title, string message);

        Task<bool> ShowCustomDialog(string title, string message, string button1, string button2);

        string ShowDatabaseFileDialog();

        string ShowFolderDialog();

        void ShowPrintDialog(PrintDocument document);
    }
}