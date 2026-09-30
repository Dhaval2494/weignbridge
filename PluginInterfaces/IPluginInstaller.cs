namespace PluginInterfaces
{
    public interface IPluginInstaller
    {
        //1. Should install supporting DB Schema
        //2. Should install supporting Windows Services
        void InstallPlugin();

        //1. Set IsEnabled = true in PluginManagement DB
        //2. Should Start supporting Windows Services.
        void EnablePlugin();

        //1. Set IsEnabled = false in PluginManagement DB
        //2. Should Stop supporting Windows Services.
        void DisablePlugin();

        //1. Should remove relevant DB Schema
        //2. Should stop and remove supporting Windows Services
        void UnInstallPlugin();
    }
}
