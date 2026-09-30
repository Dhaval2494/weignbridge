using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WeighingSolution.Helpers
{
    public class PluginHelper
    {
        public static Page FindParentPage(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            //CHeck if this is the end of the tree
            if (parent == null) return null;

            Page parentPage = parent as Page;
            if (parentPage != null)
            {
                return parentPage;
            }
            else
            {
                //use recursion until it reaches a Page
                return FindParentPage(parent);
            }
        }
        public static Window FindParentWindow(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            //CHeck if this is the end of the tree
            if (parent == null) return null;

            Window parentWindow = parent as Window;
            if (parentWindow != null)
            {
                return parentWindow;
            }
            else
            {
                //use recursion until it reaches a window
                return FindParentWindow(parent);
            }
        }

    }
}
