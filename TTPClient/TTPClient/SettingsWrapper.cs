using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TTPClient.Properties;

namespace TTPClient
{
    public class SettingsWrapper : SettingsWrapperI
    {
        public void setEmail(string email)
        {
            throw new NotImplementedException();
        }

        public void setTTP(string ttp)
        {
            throw new NotImplementedException();
        }

        public string loadTTP()
        {
            throw new NotImplementedException();
        }

        public string loadEmail()
        {
            throw new NotImplementedException();
        }

        public bool isSet()
        {
            throw new NotImplementedException();
        }

        public  bool regWithTracker()
        {
            var email = (string)Settings.Default["Email"];
            var ttp = (string)Settings.Default["TTP"];

            return regWithTracker(email, ttp);
        }

        public   bool regWithTracker(string email, string tracker)
        {
            try
            {
                byte[] emailBytes = System.Text.Encoding.UTF8.GetBytes(email);
                var url = tracker + "/rest/sessions/";
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
