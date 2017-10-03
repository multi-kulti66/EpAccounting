// ///////////////////////////////////
// File: SelectAllTextOnFocusBehavior.cs
// Last Change: 02.09.2017  10:30
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Behavior
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;



    public class SelectAllTextOnFocusBehavior : Behavior<TextBox>
    {
        #region Fields

        public static readonly DependencyProperty SelectTextOnFocusProperty = DependencyProperty
                .RegisterAttached("SelectTextOnFocus",
                                  typeof(bool),
                                  typeof(SelectAllTextOnFocusBehavior),
                                  new FrameworkPropertyMetadata(false, GotFocus));

        #endregion



        public static void SetSelectTextOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectTextOnFocusProperty, value);
        }

        private static void GotFocus(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textbox = d as TextBox;

            if (null == textbox)
            {
                return;
            }

            if ((e.NewValue as bool?).GetValueOrDefault((false)))
            {
                textbox.GotKeyboardFocus += OnKeyboardFocusSelectText;
            }
            else
            {
                textbox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
            }
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                ((TextBox)sender).SelectAll();
            }
        }
    }
}