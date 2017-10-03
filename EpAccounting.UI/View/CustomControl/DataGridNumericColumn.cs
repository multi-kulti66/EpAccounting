// ///////////////////////////////////
// File: DataGridNumericColumn.cs
// Last Change: 02.09.2017  11:21
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.View.CustomControl
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;



    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            edit.PreviewTextInput += this.Edit_PreviewTextInput;
            DataObject.AddPastingHandler(edit, this.OnPaste);
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            object data = e.SourceDataObject.GetData(DataFormats.Text);

            if (!this.IsDataValid(data))
            {
                e.CancelCommand();
            }
        }

        private void Edit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !this.IsDataValid(e.Text);
        }

        private bool IsDataValid(object data)
        {
            try
            {
                Convert.ToInt32(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}