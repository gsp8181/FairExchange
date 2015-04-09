using System;
using System.Diagnostics;
using System.Net;
using FEClient.Security;
using Grapevine;
using Grapevine.Server;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global

namespace FEClient.API
{

        public sealed partial class ClientRestApi : RESTResource
        {


            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/file/?$")]
            public void HandleFileRecievedRequest(HttpListenerContext context)
            {

                var x = GetJsonPayload(context.Request);
#if TRACE
                Debug.WriteLine("/file/ " + x);
#endif

                string filename, email, data, signature, guid, iv;
                try
                {
                    filename = x.Value<string>("fileName");
                    email = x.Value<string>("email"); //TODO: find off tracker or reject
                    data = x.Value<string>("data"); //TODO: encrypted
                    guid = x.Value<string>("guid");
                    signature = x.Value<string>("signature");
                    iv = x.Value<string>("iv");
                }
                catch (NullReferenceException)
                {
                    JObject eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    #if TRACE
                    Debug.WriteLine("/file/ SENT " + eresponse);
#endif
                    SendJsonResponse(context, eresponse);
                    return;
                }
                FileSend fs = new FileSend {email = email, fileName = filename, iv = iv, guid=guid}; //TODO: tidy up, dont reuse?
                //this.SendTextResponse(context, x);
                NotifyArgs args = new NotifyArgs();
                FileRecieved(this, fs, args);
                if (args.hasSet)
                {
                    var sig = Rsa.SignData(Base64.Base64Decode(data));
                    JObject response = new JObject {{"accepted", true}, {"signature", sig}};
                    #if TRACE
                    Debug.WriteLine("/file/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                    fs.data = data;
                    FileRecievedAndRespSent(this, fs); 
                }
                else
                {
                    JObject response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    #if TRACE
                    Debug.WriteLine("/file/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                }
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/notify/?$")]
            public void HandleNotifyRecievedRequest(HttpListenerContext context)
            {
                #if TRACE
                Debug.WriteLine("/notify/ (cont)");
#endif
                var vars = NotifySend(context);
                if (vars == null)
                    return;

                JObject response = new JObject {{"accepted", true}}; //TODO: email
                #if TRACE
                Debug.WriteLine("/notify/ SENT " + response);
#endif
                SendJsonResponse(context, response);
                NotifyRecieved(this, vars);

            }

            private NotifyRequest NotifySend(HttpListenerContext context)//TODO: rename
            {
                var jsonStr = GetJsonPayload(context.Request); //TODO: validate here with Json.NET Schema
                #if TRACE
                Debug.WriteLine(jsonStr);
#endif
                string fileName, email, guid, port;
                int timeout, complexity;
                try
                {
                    fileName = jsonStr.Value<string>("fileName"); //TODO: make sure NOT NULL
                    email = jsonStr.Value<string>("email");
                    guid = jsonStr.Value<string>("guid");
                    timeout = jsonStr.Value<int>("timeout");
                    complexity = jsonStr.Value<int>("complexity");
                    port = jsonStr.Value<string>("port");
                }
                catch (NullReferenceException)
                {
                    JObject eresponse = new JObject { { "accepted", false }, { "error", "malformed JSON" } };
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    #if TRACE
                    Debug.WriteLine("/notify/ OR /start/ SENT " + eresponse);
#endif
                    SendJsonResponse(context, eresponse);
                    return null;
                }
                catch (FormatException)
                {
                    JObject eresponse = new JObject { { "accepted", false }, { "error", "malformed JSON" } };
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    #if TRACE
                    Debug.WriteLine("/notify/ OR /start/ SENT " + eresponse);
#endif
                    SendJsonResponse(context, eresponse);
                    return null;
                }

                var ip = context.Request.RemoteEndPoint.Address + ":" + port;

                var output = new NotifyRequest {email = email, fileName = fileName, ip = ip, guid=guid, timeout=timeout, complexity=complexity};
                //TODO: err?
                return output;
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/key/?$")]
            public void HandleKeySend(HttpListenerContext context)
            {
                JObject args = GetJsonPayload(context.Request);
                #if TRACE
                Debug.WriteLine("/key/ " + args);
#endif
                var kArgs = new KeyArgs();
                kArgs.guid = args.Value<string>("guid");
                kArgs.i = args.Value<int>("i");
                kArgs.key = args.Value<string>("key");
                var callback = new NotifyArgs();
                KeyRecieved(this, kArgs, callback);

                if (callback.hasSet)
                {
                    JObject response = new JObject { { "accepted", true }, { "signature", "sig" } }; //TODO: implement
                    #if TRACE
                    Debug.WriteLine("/key/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                }
                else
                {
                    JObject response = new JObject { { "accepted", false }, { "error", "cancelled" } };
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    #if TRACE
                    Debug.WriteLine("/key/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                }

            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/start/?$")]
            public void HandleStartTransmissionRequest(HttpListenerContext context)
            {
                #if TRACE
                Debug.WriteLine("/start/ cont");
#endif
                NotifyArgs args = new NotifyArgs();
                var vars = NotifySend(context);
                if (vars == null)
                    return;

                StartTransmission(this, vars, args);
                if (args.hasSet)
                {
                    JObject response = new JObject {{"accepted", true}};
                    #if TRACE
                    Debug.WriteLine("/start/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                    StartTransmissionAndRespSent(this, vars);
                }
                else
                {
                    JObject response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    #if TRACE
                    Debug.WriteLine("/start/ SENT " + response);
#endif
                    SendJsonResponse(context, response);
                }
            }

            [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/key/?$")]
            public void HandleGetKeyRequest(HttpListenerContext context)
            {//TODO: add debug
                SendTextResponse(context, Rsa.getPublicKey());
            }

            [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/ident/?$")]
            public void HandleIdentRequest(HttpListenerContext context)
            {//TODO: add debug
#if TRACE
                Debug.WriteLine("/ident/");
#endif
                var returnObj = new JObject();
                returnObj.Add("email", SettingsWrapper.Instance.Email);
                returnObj.Add("pubKey", Rsa.getPublicKey());
                Debug.WriteLine("/ident/ sent " + returnObj);
                SendJsonResponse(context, returnObj);
            }


            [RESTRoute]
            public void HandleAllGetRequests(HttpListenerContext context)
            {
                SendTextResponse(context, "GET is a success!"); //TODO: change to api description
            }
        }
}
