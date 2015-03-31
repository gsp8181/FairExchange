using Grapevine;
using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TTPClient
{

        public sealed partial class MyResource : RESTResource
        {


            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/file/?$")]
            public void HandleFileRecievedRequest(HttpListenerContext context)
            {
                var x = this.GetJsonPayload(context.Request);
                string filename, email, data;
                try
                {
                    filename = x.Value<String>("fileName");
                    email = x.Value<String>("email"); //TODO: find off tracker or reject
                    data = x.Value<String>("data");
                }
                catch (NullReferenceException e)
                {
                    JObject eresponse = new JObject();
                    eresponse.Add("accepted", false);
                    eresponse.Add("error", "malformed JSON");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    this.SendJsonResponse(context, eresponse);
                    return;
                }
                FileSend fs = new FileSend();
                fs.data = Security.Base64Decode(data);
                fs.email = email;
                fs.fileName = filename;
                //this.SendTextResponse(context, x);
                NotifyArgs args = new NotifyArgs();
                FileRecieved(this, fs, args);
                if (args.hasSet)
                {
                    JObject response = new JObject();
                    response.Add("accepted", true); //TODO: sign!!
                    this.SendJsonResponse(context, response);
                }
                else
                {
                    JObject response = new JObject();
                    response.Add("accepted", false);
                    response.Add("error", "cancelled");
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

                JObject response = new JObject();
                response.Add("accepted", true);
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
                    JObject eresponse = new JObject();
                    eresponse.Add("accepted", false);
                    eresponse.Add("error", "malformed JSON");
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    this.SendJsonResponse(context, eresponse);
                    return null;
                }
                var ip = context.Request.RemoteEndPoint.Address.ToString();

                var output = new NotifyRequest();
                output.email = email; //TODO: err?
                output.fileName = fileName;
                output.ip = ip;
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
                    JObject response = new JObject();
                    response.Add("accepted", true);
                    this.SendJsonResponse(context, response);
                }
                else
                {
                    JObject response = new JObject();
                    response.Add("accepted", false);
                    response.Add("error", "cancelled");
                    context.Response.StatusCode = (int)HttpStatusCode.Gone;
                    this.SendJsonResponse(context, response);
                }
            }


            [RESTRoute]
            public void HandleAllGetRequests(HttpListenerContext context)
            {
                this.SendTextResponse(context, "GET is a success!");
            }
        }
}
