﻿using Grapevine;
using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq; 
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using TTPClient.Security;

namespace TTPClient
{

        public sealed partial class ClientRestApi : RESTResource
        {


            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/file/?$")]
            public void HandleFileRecievedRequest(HttpListenerContext context)
            {

                var x = this.GetJsonPayload(context.Request);
                Debug.WriteLine("/file/ " + x);

                string filename, email, data, signature;
                try
                {
                    filename = x.Value<string>("fileName");
                    email = x.Value<string>("email"); //TODO: find off tracker or reject
                    data = x.Value<string>("data"); //TODO: encrypted
                    signature = x.Value<string>("signature");
                }
                catch (NullReferenceException)
                {
                    JObject eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    this.SendJsonResponse(context, eresponse);
                    return;
                }
                FileSend fs = new FileSend {email = email, fileName = filename}; //TODO: tidy up, dont reuse?
                //this.SendTextResponse(context, x);
                NotifyArgs args = new NotifyArgs();
                FileRecieved(this, fs, args);
                if (args.hasSet)
                {
                    var sig = Rsa.SignData(Base64.Base64Decode(data));
                    JObject response = new JObject {{"accepted", true}, {"signature", sig}};
                    this.SendJsonResponse(context, response);
                    fs.data = data;
                    FileRecievedAndRespSent(this, fs); 
                }
                else
                {
                    JObject response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    this.SendJsonResponse(context, response);
                }
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/notify/?$")]
            public void HandleNotifyRecievedRequest(HttpListenerContext context)
            {
                var vars = NotifySend(context);
                if (vars == null)
                    return;

                JObject response = new JObject {{"accepted", true}};
                this.SendJsonResponse(context, response);
                NotifyRecieved(this, vars);

            }

            private NotifyRequest NotifySend(HttpListenerContext context)//TODO: rename
            {
                var jsonStr = this.GetJsonPayload(context.Request); //TODO: validate here with Json.NET Schema
                string fileName, email;
                try
                {
                    fileName = jsonStr.Value<String>("fileName");
                    email = jsonStr.Value<String>("email"); //TODO: find off tracker or reject
                }
                catch (NullReferenceException e)
                {
                    JObject eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    this.SendJsonResponse(context, eresponse);
                    return null;
                }
                var ip = context.Request.RemoteEndPoint.Address.ToString();

                var output = new NotifyRequest {email = email, fileName = fileName, ip = ip};
                //TODO: err?
                return output;
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/start/?$")]
            public void HandleStartTransmissionRequest(HttpListenerContext context)
            {
                NotifyArgs args = new NotifyArgs();
                var vars = NotifySend(context);
                if (vars == null)
                    return;

                StartTransmission(this, vars, args);
                if (args.hasSet)
                {
                    JObject response = new JObject {{"accepted", true}};
                    this.SendJsonResponse(context, response);
                    StartTransmissionAndRespSent(this, vars);
                }
                else
                {
                    JObject response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    this.SendJsonResponse(context, response);
                }
            }

            [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/key/?$")]
            public void HandleGetKeyRequest(HttpListenerContext context)
            {
                this.SendTextResponse(context, Rsa.getPublicKey());
            }

#if DEBUG
            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/decrypt/?$")]
            public void HandleDecryptRequest(HttpListenerContext context)
            {
                var payload = JObject.Parse(this.GetPayload(context.Request));
                var data = payload.Value<string>("data");
                var key = payload.Value<string>("key");
                context.Response.ContentType = "text/plain";
                this.SendTextResponse(context, Rsa.DecryptData(data,key));
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/encrypt/?$")]
            public void HandleEncryptRequest(HttpListenerContext context)
            {
                //JObject response = new JObject();
                //response.Add("Key",key);
                //response.Add("data", data);
                this.SendJsonResponse(context,Rsa.EncryptData(this.GetPayload(context.Request)));
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/sign/?$")]
            public void HandleSignRequest(HttpListenerContext context)
            {
                context.Response.ContentType = "text/plain";
                this.SendTextResponse(context,Rsa.SignData(this.GetPayload(context.Request)));
            }
            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/encryptTo/?$")]
            public void HandleEncryptToRequest(HttpListenerContext context)
            {
                var payload = JObject.Parse(this.GetPayload(context.Request));
                var data = payload.Value<string>("data");
                var key = payload.Value<string>("key");
                this.SendJsonResponse(context, Rsa.EncryptData(data,key));
            }
#endif


            [RESTRoute]
            public void HandleAllGetRequests(HttpListenerContext context)
            {
                this.SendTextResponse(context, "GET is a success!");
            }
        }
}