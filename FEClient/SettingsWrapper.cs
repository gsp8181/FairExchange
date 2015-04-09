using FEClient.Properties;

namespace FEClient
{
    public class SettingsWrapper
    {
        private static SettingsWrapper _SettingsWrapper = new SettingsWrapper();
        private SettingsWrapper() { }
        public static SettingsWrapper Instance {get { return _SettingsWrapper; }}

        public string Email
        {
            get { return (string)Settings.Default["Email"]; }
            set
            {
                Settings.Default["Email"] = value;
                Settings.Default.Save();
            }
        }

        public bool IsSet
        {
            get { return (!string.IsNullOrWhiteSpace(Email)); }

        }
    }
}
