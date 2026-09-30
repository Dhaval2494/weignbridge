using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WeighingSolution.UserControls
{
    //https://social.msdn.microsoft.com/Forums/sqlserver/en-US/342dd372-bbcd-4f91-b178-8dafff582c15/cannot-set-autocompletebox-isdropdownopen-property?forum=silverlightcontrols
    //http://timheuer.com/blog/archive/2008/11/05/silverlight-editable-combobox-using-styles.aspx
    //http://www.broculos.net/2014/04/wpf-autocompletebox-autocomplete-text.html
    public class AutoCompleteComboBox : AutoCompleteBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ToggleButton toggle = (ToggleButton)GetTemplateChild("DropDownToggle");
            toggle.Click += Toggle_Click;
            //ItemFilter = FilterAutoComplete;
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
            if (IsDropDownOpen)
            {
                ((TextBox)GetTemplateChild("Text")).SelectAll();
            }
            Focus();
        }
        //private bool FilterAutoComplete(string searchText, object item)
        //{
        //    if (string.IsNullOrEmpty(searchText))
        //    {
        //        return true;
        //    }

        //    string f = item as string;
        //    if (f == null)
        //        return false;

        //    return f.ToLower().Contains(searchText.ToLower());
        //}
    }
    public class AutoCompleteBoxEx : AutoCompleteBox
    {
        private TextBox _textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template == null) return;
            _textBox = Template.FindName("Text", this) as TextBox;
        }

        public void Select(int start, int length)
        {
            if (_textBox == null) return;
            _textBox.Select(start, length);
        }
        public void CaretIndex()
        {
            if (_textBox == null) return;
            _textBox.CaretIndex = Int32.MaxValue;
        }
    }
}
