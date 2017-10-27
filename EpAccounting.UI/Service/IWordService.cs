// ///////////////////////////////////
// File: IWordService.cs
// Last Change: 25.10.2017  21:54
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Service
{
    using EpAccounting.UI.ViewModel;



    public interface IWordService
    {
        void CreateWordBill(BillItemEditViewModel billItemEditViewModel, bool visible);

        bool PrintDocument();

        void CloseDocument();
    }
}