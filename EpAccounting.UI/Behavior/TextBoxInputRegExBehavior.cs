// ///////////////////////////////////
// File: TextBoxInputRegExBehavior.cs
// Last Change: 14.03.2017  17:48
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Behavior
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;



    public class TextBoxInputRegExBehavior : Behavior<TextBox>
    {
        #region Fields

        public static readonly DependencyProperty RegularExpressionProperty =
            DependencyProperty.Register(nameof(RegularExpression), typeof(string), typeof(TextBoxInputRegExBehavior), new FrameworkPropertyMetadata(".*"));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(TextBoxInputRegExBehavior), new FrameworkPropertyMetadata(int.MinValue));

        public static readonly DependencyProperty EmptyValueProperty =
            DependencyProperty.Register(nameof(EmptyValue), typeof(string), typeof(TextBoxInputRegExBehavior), null);

        #endregion



        #region Properties

        public string RegularExpression
        {
            get { return (string)this.GetValue(RegularExpressionProperty); }
            set { this.SetValue(RegularExpressionProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        public string EmptyValue
        {
            get { return (string)this.GetValue(EmptyValueProperty); }
            set { this.SetValue(EmptyValueProperty, value); }
        }

        #endregion



        /// <summary>
        ///     Attach our behaviour. Add event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewTextInput += this.PreviewTextInputHandler;
            this.AssociatedObject.PreviewKeyDown += this.PreviewKeyDownHandler;
            DataObject.AddPastingHandler(this.AssociatedObject, this.PastingHandler);
        }

        /// <summary>
        ///     Deattach our behaviour. remove event handlers
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PreviewTextInput -= this.PreviewTextInputHandler;
            this.AssociatedObject.PreviewKeyDown -= this.PreviewKeyDownHandler;
            DataObject.RemovePastingHandler(this.AssociatedObject, this.PastingHandler);
        }



        #region Event handlers [PRIVATE] --------------------------------------

        private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
        {
            string text;

            if (this.AssociatedObject.Text.Length < this.AssociatedObject.CaretIndex)
            {
                text = this.AssociatedObject.Text;
            }
            else
            {
                //  Remaining text after removing selected text.

                text = this.TreatSelectedText(out string remainingTextAfterRemoveSelection)
                           ? remainingTextAfterRemoveSelection.Insert(this.AssociatedObject.SelectionStart, e.Text)
                           : this.AssociatedObject.Text.Insert(this.AssociatedObject.CaretIndex, e.Text);
            }

            e.Handled = !this.ValidateText(text);
        }

        /// <summary>
        ///     PreviewKeyDown event handler
        /// </summary>
        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.EmptyValue))
            {
                return;
            }

            string text = null;

            // Handle the Backspace key
            if (e.Key == Key.Back && (!this.TreatSelectedText(out text) && this.AssociatedObject.SelectionStart > 0))
            {
                text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart - 1, 1);
            }

            // Handle the Delete key
            if (e.Key == Key.Delete && this.AssociatedObject.Text.Length > this.AssociatedObject.SelectionStart && (!this.TreatSelectedText(out text)))
            {
                // Otherwise delete next symbol
                text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart, 1);
            }

            if (text == string.Empty)
            {
                this.AssociatedObject.Text = this.EmptyValue;
                if (e.Key == Key.Back)
                {
                    this.AssociatedObject.SelectionStart++;
                }
                e.Handled = true;
            }
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text));

                if (!this.ValidateText(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        #endregion Event handlers [PRIVATE] -----------------------------------



        #region Auxiliary methods [PRIVATE] -----------------------------------

        /// <summary>
        ///     Validate certain text by our regular expression and text length conditions
        /// </summary>
        /// <param name="text"> Text for validation </param>
        /// <returns> True - valid, False - invalid </returns>
        private bool ValidateText(string text)
        {
            return (new Regex(this.RegularExpression, RegexOptions.IgnoreCase)).IsMatch(text) && (this.MaxLength == 0 || text.Length <= this.MaxLength);
        }

        /// <summary>
        ///     Handle text selection
        /// </summary>
        /// <returns>true if the character was successfully removed; otherwise, false. </returns>
        private bool TreatSelectedText(out string text)
        {
            text = null;
            if (this.AssociatedObject.SelectionLength <= 0)
            {
                return false;
            }

            int length = this.AssociatedObject.Text.Length;
            if (this.AssociatedObject.SelectionStart >= length)
            {
                return true;
            }

            if (this.AssociatedObject.SelectionStart + this.AssociatedObject.SelectionLength >= length)
            {
                this.AssociatedObject.SelectionLength = length - this.AssociatedObject.SelectionStart;
            }

            text = this.AssociatedObject.Text.Remove(this.AssociatedObject.SelectionStart, this.AssociatedObject.SelectionLength);
            return true;
        }

        #endregion Auxiliary methods [PRIVATE] --------------------------------
    }
}