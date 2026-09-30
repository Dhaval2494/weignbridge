using System;
using System.Collections.Generic;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WeighingSolution.ViewModel;


namespace WeighingSolution.View
{
    public partial class ApiIntegrationMaster_Page : Page
    {
        private readonly ApiIntegrationMasterViewModel viewModel;

        // Bound by the Params tab's DataGridComboBoxColumn (Query / Path).
        // Static so it can be referenced from XAML via x:Static without an extra converter.
        public static List<string> LocationOptions { get; } = new List<string> { "Query", "Path" };

        public ApiIntegrationMaster_Page()
        {
            InitializeComponent();

            viewModel = new ApiIntegrationMasterViewModel();
            DataContext = viewModel;
        }

        private Point _fieldDragStartPoint;

        private void FieldsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _fieldDragStartPoint = e.GetPosition(null);
        }

        private void FieldsList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Point current = e.GetPosition(null);
            Vector diff = _fieldDragStartPoint - current;
            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            var listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
            var field = listBoxItem?.DataContext as TemplateFieldItem;
            if (field == null)
                return;

            DragDrop.DoDragDrop(listBoxItem, new DataObject("TemplateField", field.Placeholder), DragDropEffects.Copy);
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T match) return match;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void txtRawBodyTemplate_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent("TemplateField") ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }
        private void txtRawBodyTemplate_PreviewDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TemplateField")) return;
            string placeholder = (string)e.Data.GetData("TemplateField");
            var textBox = (TextBox)sender;
            int dropIndex = textBox.GetCharacterIndexFromPoint(e.GetPosition(textBox), true);
            if (dropIndex < 0) dropIndex = textBox.Text.Length;
            textBox.Text = textBox.Text.Insert(dropIndex, placeholder);
            textBox.CaretIndex = dropIndex + placeholder.Length;
            textBox.Focus();
            e.Handled = true;
        }

        // Fallback drop zone covering the WHOLE editor area (raw textbox + form-data grid +
        // splitter + field panel), not just the raw-JSON TextBox. Without this, dragging shows
        // a permanent "blocked" cursor whenever Body Mode isn't set to a raw format, since a
        // Collapsed TextBox can't receive drag events at all - there'd be no valid drop target
        // anywhere on the page.
        private void BodyEditorArea_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent("TemplateField") ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }
        private void BodyEditorArea_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TemplateField")) return;

            if (txtRawBodyTemplate.Visibility == Visibility.Visible)
            {
                txtRawBodyTemplate_PreviewDrop(txtRawBodyTemplate, e);
                return;
            }

            // Raw editor not visible (form-data mode) - append to the end of the raw template
            // rather than silently losing the drop; visible once they switch back to raw mode.
            string placeholder = (string)e.Data.GetData("TemplateField");
            if (viewModel.SelectedIntegration != null)
                viewModel.SelectedIntegration.RawBodyTemplate = (viewModel.SelectedIntegration.RawBodyTemplate ?? string.Empty) + placeholder;

            e.Handled = true;
        }
        private void FieldsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
            var field = listBoxItem?.DataContext as TemplateFieldItem;
            if (field == null) return;

            InsertPlaceholderAtCaret(field.Placeholder);
            e.Handled = true;
        }

        // Inserts at wherever the user last placed their cursor in the raw body editor.
        // Click into the body to position the caret, THEN double-click a field here - the
        // caret index survives losing focus to the side panel, so this always lands exactly
        // where intended (unlike screen-position-based drag/drop with scrolled/monospace text).
        private void InsertPlaceholderAtCaret(string placeholder)
        {
            if (txtRawBodyTemplate.Visibility != Visibility.Visible)
            {
                // Body isn't in raw mode - nowhere sensible to place a caret, just append.
                if (viewModel.SelectedIntegration != null)
                    viewModel.SelectedIntegration.RawBodyTemplate = (viewModel.SelectedIntegration.RawBodyTemplate ?? string.Empty) + placeholder;
                return;
            }

            int caretIndex = txtRawBodyTemplate.CaretIndex;
            string text = txtRawBodyTemplate.Text ?? string.Empty;
            if (caretIndex < 0 || caretIndex > text.Length) caretIndex = text.Length;

            txtRawBodyTemplate.Text = text.Insert(caretIndex, placeholder);
            txtRawBodyTemplate.CaretIndex = caretIndex + placeholder.Length;
            txtRawBodyTemplate.Focus();
        }



        // When the user switches Auth Type, rebuild the AuthFields collection
        // to match whatever fields that mode needs (Key/Value/AddTo for ApiKey,
        // Username/Password for BasicAuth, etc.) - see ViewModel.RegenerateAuthFields.
        private void cboAuthMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.RegenerateAuthFields();
        }
    }
}
