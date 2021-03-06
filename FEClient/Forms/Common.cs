﻿using System.Net;
using System.Windows.Forms;
using FEClient.Database;
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace FEClient.Forms
{
    internal static class Common
    {
        public static Ident GetSshKey(string ip)
        {
            var client = new RESTClient("http://" + ip);

            var keyReq = new RESTRequest("/ident/");

            var keyResponse = client.Execute(keyReq);

            if (keyResponse.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Could not contact " + ip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var errResponse = new Ident(false, null, null);
                return errResponse;
            }

            var keyRespObj = JObject.Parse(keyResponse.Content);

            var remoteKey = keyRespObj.Value<string>("pubKey"); 
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
                    return new Ident(false, remoteKey, email);
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
                    return new Ident(false, remoteKey, email);
                }
            }
            return new Ident(true, remoteKey, email);
        }
    }
}