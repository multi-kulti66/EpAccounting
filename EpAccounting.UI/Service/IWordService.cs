// ///////////////////////////////////
// File: IWordService.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.Service
{
    using ViewModel;


    public interface IWordService
    {
        void CreateWordBill(BillItemEditViewModel billItemEditViewModel, bool visible);

        bool PrintDocument();

        void CloseDocument();
    }
}