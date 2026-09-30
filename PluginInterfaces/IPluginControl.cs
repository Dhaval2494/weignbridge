using System.Windows;

namespace PluginInterfaces
{
    public interface IPluginControl
    {
        void SetupPlugin();
        void SetupPlugin(UIElement page);
        void DisposePlugin();
    }
}
