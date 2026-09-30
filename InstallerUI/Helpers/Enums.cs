namespace InstallerUI.Helpers
{
    public enum BurnInstallationState
    {
        Present,
        NotPresent
    }
    public enum InstallationMode
    {
        Undefined,
        Install,
        Uninstall,
        Repair,
        Cancel
    }
    public enum InstallationResult
    {
        Unknown,
        Success,
        SuccessRestartRequired,
        Canceled,
        Error
    }
}
