using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FEClient.API.Events;
using FEClient.Security;
using Grapevine;
using Grapevine.Server;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global

namespace FEClient.API
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed partial class ClientRestApi : RESTResource
    {
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/file/?$")]
        public void HandleFileRecievedRequest(HttpListenerContext context)
        {
            //var payload = GetJsonPayload(context.Request);
            var payload = decryptRequest(context);

#if TRACE
            Debug.WriteLine("/file/ " + payload);
#endif

            string filename, email, data, guid, iv, signature, jobj;
            try
            {
                jobj = payload.Value<string>("data"); //TODO: pass through as event
                var x = JObject.Parse(jobj); //TODO: could be all in one
                signature = payload.Value<string>("signature");


                filename = x.Value<string>("fileName");
                email = x.Value<string>("email"); //TODO: find off tracker or reject
                data = x.Value<string>("data"); //TODO: encrypted
                guid = x.Value<string>("guid");
                //signature = x.Value<string>("signature");
                iv = x.Value<string>("iv");
            }
            catch (NullReferenceException)
            {
                var eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
#if TRACE
                Debug.WriteLine("/file/ SENT " + eresponse);
#endif
                SendJsonResponse(context, eresponse);
                return;
            }
            var fs = new FileSendEventArgs(filename,email,jobj,iv,guid,signature);
            FileRecieved(this, fs);
            if (fs.HasSet)
            {
                var sig = Rsa.SignStringData(signature);
                var response = new JObject {{"accepted", true}, {"signature", sig}};
#if TRACE
                Debug.WriteLine("/file/ SENT " + response);
#endif
                SendJsonResponse(context, response);
                var fs2 = new FileSendEventArgs(filename, email, data, iv, guid, signature);
                FileRecievedAndRespSent(this, fs2);
            }
            else
            {
                var response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                context.Response.StatusCode = (int) HttpStatusCode.Gone;
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

            var response = new JObject {{"accepted", true}}; //TODO: email
#if TRACE
            Debug.WriteLine("/notify/ SENT " + response);
#endif
            SendJsonResponse(context, response);
            NotifyRecieved(this, vars);
        }

        private NotifyRequestEventArgs NotifySend(HttpListenerContext context)
        {
            var jsonStr = decryptRequest(context);

            //var jsonStr = GetJsonPayload(context.Request);
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
                var eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
#if TRACE
                Debug.WriteLine("/notify/ OR /start/ SENT " + eresponse);
#endif
                SendJsonResponse(context, eresponse);
                return null;
            }
            catch (FormatException)
            {
                var eresponse = new JObject {{"accepted", false}, {"error", "malformed JSON"}};
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
#if TRACE
                Debug.WriteLine("/notify/ OR /start/ SENT " + eresponse);
#endif
                SendJsonResponse(context, eresponse);
                return null;
            }

            var ip = context.Request.RemoteEndPoint.Address + ":" + port;

            var output = new NotifyRequestEventArgs(fileName, email, ip, guid, timeout, complexity);

            //TODO: err?
            return output;
        }

        private JObject decryptRequest(HttpListenerContext context) //TODO: if encrypted
        {
            var str = GetJsonPayload(context.Request);
            string key, data;
            try
            {
                key = str.Value<string>("key"); //TODO: this may trigger for one or two things it shouldn't
                data = str.Value<string>("data");
               
            }
            catch (Exception)
            {
                return str;

            }
            try
            {
                var decrypted = Rsa.DecryptData(data, key);
                var decryptedStr = Encoding.UTF8.GetString(decrypted);


                var jsonStr = JObject.Parse(decryptedStr);
                return jsonStr;
            }
            catch (Exception)
            {
                var eresponse = new JObject {{"accepted", false}, {"error", "could not decrypt"}};
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
#if TRACE
                Debug.WriteLine("decrypt SENT " + eresponse);
#endif
                SendJsonResponse(context, eresponse);
                return null; //TODO: for all if null, quit
            }
            

        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/key/?$")]
        public void HandleKeySend(HttpListenerContext context)
        {
            var args = decryptRequest(context);
            //var args = GetJsonPayload(context.Request);
#if TRACE
            Debug.WriteLine("/key/ " + args);
#endif
            var guid = args.Value<string>("guid");
            var i = args.Value<int>("i");
            var key = args.Value<string>("key");
            var kArgs = new KeyReceivedEventArgs(key, guid, i);
            KeyRecieved(this, kArgs);

            if (kArgs.HasSet)
            {
                var response = new JObject {{"accepted", true}, {"signature", Rsa.SignStringData(args.ToString())}};
#if TRACE
                Debug.WriteLine("/key/ SENT " + response);
#endif
                SendJsonResponse(context, response);
            }
            else
            {
                var response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                context.Response.StatusCode = (int) HttpStatusCode.Gone;
#if TRACE
                Debug.WriteLine("/key/ SENT " + response);
#endif
                SendJsonResponse(context, response);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/start/?$")]
        public void HandleStartTransmissionRequest(HttpListenerContext context)
        {
#if TRACE
            Debug.WriteLine("/start/ cont");
#endif
            var nrArgs = NotifySend(context);
            if (nrArgs == null)
                return;

            var vars = new StartTransmissionEventArgs(nrArgs);

            StartTransmission(this, vars);
            if (vars.HasSet)
            {
                var response = new JObject {{"accepted", true}};
#if TRACE
                Debug.WriteLine("/start/ SENT " + response);
#endif
                SendJsonResponse(context, response);
                StartTransmissionAndRespSent(this, vars);
            }
            else
            {
                var response = new JObject {{"accepted", false}, {"error", "cancelled"}};
                context.Response.StatusCode = (int) HttpStatusCode.Gone;
#if TRACE
                Debug.WriteLine("/start/ SENT " + response);
#endif
                SendJsonResponse(context, response);
            }
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/key/?$")]
        public void HandleGetKeyRequest(HttpListenerContext context)
        {
#if TRACE
            Debug.WriteLine("/key/");
            Debug.WriteLine("/key/ sent " + Rsa.PublicKey);
#endif
            SendTextResponse(context, Rsa.PublicKey);
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/ident/?$")]
        public void HandleIdentRequest(HttpListenerContext context)
        {
#if TRACE
            Debug.WriteLine("/ident/");
#endif
            var returnObj = new JObject {{"email", SettingsWrapper.Email}, {"pubKey", Rsa.PublicKey}};
#if TRACE
            Debug.WriteLine("/ident/ sent " + returnObj);
#endif
            SendJsonResponse(context, returnObj);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/finish/?$")]
        public void HandleFinishRequest(HttpListenerContext context)
        {
#if TRACE
            Debug.WriteLine("/finish/");
#endif
            //var payload = GetJsonPayload(context.Request);
            var payload = decryptRequest(context);
            var guid = payload.Value<string>("guid");
            var args = new FinishEventArgs(guid);
            Finish(this, args);
            if (args.HasSet)
            {
                var resp = new JObject { { "accepted", true } };
#if TRACE
                Debug.WriteLine("/finish/ sent " + resp);
#endif
                SendJsonResponse(context, resp);
            }
            else
            {
                var resp = new JObject { { "accepted", false } };
#if TRACE
                Debug.WriteLine("/finish/ sent " + resp);
#endif
                context.Response.StatusCode = (int) HttpStatusCode.Gone;
                SendJsonResponse(context, resp);
            }
        }

        /*[RESTRoute]
        public void HandleAllGetRequests(HttpListenerContext context)
        {
            SendTextResponse(context, "GET is a success!"); //TODO: change to api description
        }*/
    }
}