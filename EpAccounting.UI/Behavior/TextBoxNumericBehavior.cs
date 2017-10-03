// ///////////////////////////////////
// File: TextBoxNumericBehavior.cs
// Last Change: 02.09.2017  10:30
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Behavior
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;



    public class TextBoxNumericBehavior : Behavior<TextBox>
    {
        #region Fields

        public static readonly DependencyProperty IsTextBoxNumericEnabledProperty =
                DependencyProperty.RegisterAttached("IsTextBoxNumericEnabled",
                                                    typeof(bool),
                                                    typeof(TextBoxNumericBehavior),
                                                    new UIPropertyMetadata(false, OnValueChanged));

        #endregion



        public static bool GetIsTextBoxNumericEnabled(Control control)
        {
            return (bool)control.GetValue(IsTextBoxNumericEnabledProperty);
        }

        public static void SetIsTextBoxNumericEnabled(Control control, bool value)
        {
            control.SetValue(IsTextBoxNumericEnabledProperty, value);
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Control uiElement = dependencyObject as Control;

            if (uiElement == null)
            {
                return;
            }

            if (e.NewValue is bool && (bool)e.NewValue)
            {
                uiElement.PreviewTextInput += OnTextInput;
                uiElement.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(uiElement, OnPaste);
            }

            else
            {
                uiElement.PreviewTextInput -= OnTextInput;
                uiElement.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(uiElement, OnPaste);
            }
        }

        private static void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(c => !char.IsDigit(c)))
            {
                e.Handled = true;
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text)).Trim();
                if (text.Any(c => !char.IsDigit(c)))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}