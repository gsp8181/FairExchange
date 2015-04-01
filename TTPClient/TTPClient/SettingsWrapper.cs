﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TTPClient.Properties;

namespace TTPClient
{
    public interface SettingsWrapperI //TODO: getInstance()
    {
        string Email { get; set; }
        string TTP { get; set; }
        bool IsSet { get; }

        bool RegWithTracker();

        bool RegWithTracker(string email, string ttp);
    }

    public class SettingsWrapper : SettingsWrapperI
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
            get { return (!(string.IsNullOrWhiteSpace(TTP) || string.IsNullOrWhiteSpace(Email))); }

        }

        public bool RegWithTracker() //TODO: IS SET
        {
            return RegWithTracker(Email, TTP);
        }

        public bool RegWithTracker(string email, string ttp)
        {
            try
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
            }
        }
    }
}