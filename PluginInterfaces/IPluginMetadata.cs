namespace PluginInterfaces
{
    public interface IPluginMetadata
    {
        string PluginName { get; }
        //Free/Paid
        string PluginType { get; }
        string Description { get; }
        string MinCompatibleSoftwareVersion { get; }
        string PluginSignature { get; }
        string CompatibleScreenLayout { get; }
    }
}
