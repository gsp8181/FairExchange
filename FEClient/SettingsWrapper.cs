using FEClient.Properties;

namespace FEClient
{
    public static class SettingsWrapper
    {
        public static string Email
        {
            get { return (string) Settings.Default["Email"]; }
            set
            {
                Settings.Default["Email"] = value;
                Settings.Default.Save();
            }
        }

        public static bool IsSet
        {
            get { return (!string.IsNullOrWhiteSpace(Email)); }
        }
    }
}