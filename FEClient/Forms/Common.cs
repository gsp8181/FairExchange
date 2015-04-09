using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using FEClient.SQLite;
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace FEClient.Forms
{
    static class Common
    {
        public static bool GetValue(string ip, out string remoteKey)
        {
            RESTClient client = new RESTClient("http://" + ip);

            var keyReq = new RESTRequest("/ident/");

            var keyResponse = client.Execute(keyReq);

            if (keyResponse.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("error"); //TODO: make better
            }

            var keyRespObj = JObject.Parse(keyResponse.Content);

            remoteKey = keyRespObj.Value<string>("pubKey"); //TODO: flag on error
            var email = keyRespObj.Value<string>("email");

            var keyObj = Adapter.Instance.GetByEmail(email);

            if (keyObj == null)
            {
                var dialogResult =
                    MessageBox.Show(
                        "The key for " + email + " has not been registered, do you wish to accept?\n" + remoteKey,
                        "New key", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey();
                    dbObj.Email = email;
                    dbObj.Pem = remoteKey;
                    Adapter.Instance.Insert(dbObj);
                }
                else
                {
                    return true;
                }
            }
            else if (keyObj.Pem != remoteKey)
            {
                var dialogResult =
                    MessageBox.Show(
                        "The key for " + email +
                        " has BEEN CHANGED, this could indicate interception\n Do you wish to accept?\n" + remoteKey,
                        "CHANGED KEY", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey();
                    dbObj.Email = email;
                    dbObj.Pem = remoteKey;
                    Adapter.Instance.Insert(dbObj);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
