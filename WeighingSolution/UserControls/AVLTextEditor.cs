using ICSharpCode.AvalonEdit;
using System;
using System.ComponentModel;
using System.Windows;

namespace WeighingSolution.UserControls
{
    public class AVLTextEditor : TextEditor, INotifyPropertyChanged
    {
        public static DependencyProperty DocumentTextProperty =
            DependencyProperty.Register("DocumentText",
                                        typeof(string), typeof(AVLTextEditor),
            new PropertyMetadata((obj, args) =>
            {
                AVLTextEditor target = (AVLTextEditor)obj;
                if (target.DocumentText != (string)args.NewValue)    //avoid undo stack overflow
                    target.DocumentText = (string)args.NewValue;
            })
        );

        public string DocumentText
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(DocumentTextProperty, base.Text);
            base.OnTextChanged(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
