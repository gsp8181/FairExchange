using System.Net;
using System.Windows.Forms;
using FEClient.Database;
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace FEClient.Forms
{
    internal static class Common
    {
        public static bool GetSshKey(string ip, out string remoteKey)
        {
            var client = new RESTClient("http://" + ip);

            var keyReq = new RESTRequest("/ident/");

            var keyResponse = client.Execute(keyReq);

            if (keyResponse.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("error"); //TODO: make better
            }

            var keyRespObj = JObject.Parse(keyResponse.Content);

            remoteKey = keyRespObj.Value<string>("pubKey"); //TODO: flag on error
            var email = keyRespObj.Value<string>("email");

            var keyObj = Adapter.GetByEmail(email);

            if (keyObj == null)
            {
                var dialogResult =
                    MessageBox.Show(
                        "The key for " + email + " has not been registered, do you wish to accept?\n" + remoteKey,
                        "New key", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey {Email = email, Pem = remoteKey};
                    Adapter.Insert(dbObj);
                }
                else
                {
                    return false;
                }
            }
            else if (keyObj.Pem != remoteKey)
            {
                var dialogResult =
                    MessageBox.Show(
                        "The key for " + email +
                        " has BEEN CHANGED, this could indicate interception\nDo you wish to accept?\n" + remoteKey,
                        "CHANGED KEY", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey {Email = email, Pem = remoteKey};
                    Adapter.Insert(dbObj);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}