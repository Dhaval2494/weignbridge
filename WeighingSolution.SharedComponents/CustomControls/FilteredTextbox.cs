using System;
using System.Windows;
using System.Windows.Controls;

namespace WeighingSolution.CustomControls
{
    public class FilteredTextbox : TextBox
    {
        // Dependency Property
        public static readonly DependencyProperty FilterRegexProperty =
             DependencyProperty.Register("FilterRegex", typeof(string),
             typeof(FilteredTextbox), new FrameworkPropertyMetadata(".*"));

        // .NET Property wrapper
        [System.ComponentModel.Browsable(true)]
        public String FilterRegex
        {
            get { return (String)GetValue(FilterRegexProperty); }
            set { SetValue(FilterRegexProperty, value); }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PreviewTextInput += FilteredTextbox_PreviewTextInput;
            DataObject.AddPastingHandler(this, OnPaste);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(FilterRegex);
            if (!regex.IsMatch(tb.Text.Insert(tb.SelectionStart, text)))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        void FilteredTextbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(FilterRegex);
            if (tb.Text.Length >= tb.SelectionStart)
                e.Handled = !regex.IsMatch(tb.Text.Insert(tb.SelectionStart, e.Text));
            else
                e.Handled = !regex.IsMatch(tb.Text + e.Text);
        }
    }
}
