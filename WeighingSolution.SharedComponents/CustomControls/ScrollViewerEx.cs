using System.Windows.Controls;

namespace WeighingSolution.CustomControls
{
    public class ScrollViewerEx : ScrollViewer
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PreviewMouseWheel += ScrollViewerEx_PreviewMouseWheel;
        }

        void ScrollViewerEx_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
