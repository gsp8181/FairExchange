using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        [Obsolete]
        public string TTP
        {
            get { return (string)Settings.Default["TTP"]; }
            set
            {
                Settings.Default["TTP"] = value;
                Settings.Default.Save();
            }
        }

        public bool IsSet
        {
            get { return (string.IsNullOrWhiteSpace(Email)); }

        }

        [Obsolete]
        public bool RegWithTracker() //TODO: IS SET
        {
            return RegWithTracker(Email, TTP);
        }

        [Obsolete]
        public bool RegWithTracker(string email, string ttp)
        {
            /*try
            {
                byte[] emailBytes = System.Text.Encoding.UTF8.GetBytes(email);
                var url = ttp + "/rest/sessions/";
                var request = (HttpWebRequest)WebRequest.CreateHttp(url);
                request.Method = "POST";
                request.ContentLength = emailBytes.Length;
                request.ContentType = "application/json";
                var dataStream = request.GetRequestStream();
                dataStream.Write(emailBytes, 0, emailBytes.Length);
                dataStream.Close();
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }*/
            return true;

        }
    }
}
