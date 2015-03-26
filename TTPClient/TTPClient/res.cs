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

            [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/notify/\d+$")]
            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/bar/\D+$")]
            public void HandleFooBarRequests(HttpListenerContext context)
            {
                Notify(this,context.Request.Url.ToString());
                this.SendTextResponse(context, "foo bar is a success!");
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/file/$")]
            public void HandleFileRecievedRequest(HttpListenerContext context)
            {
                var x = this.GetPayload(context.Request);
                this.SendTextResponse(context, x);
                FileRecieved(this, x);
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/notify/$")]
            public void HandleNotifyRecievedRequest(HttpListenerContext context)
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
                    eresponse.Add("error","malformed JSON");
                    this.SendJsonResponse(context, eresponse);
                    return;
                }
                var ip = context.Request.RemoteEndPoint.Address.ToString();

                NotifyRecieved(this, ip, fileName, email);
                JObject response = new JObject();
                response.Add("accepted",true);
                this.SendJsonResponse(context,response);
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/start/$")]
            public void HandleStartTransmissionRequest(HttpListenerContext context)
            {
                NotifyArgs args = new NotifyArgs();
                StartTransmission(this, null, args);
                if (args.hasSet)
                {
                    this.SendTextResponse(context,"yayy");
                }
                else
                {
                    this.SendTextResponse(context,"nayy");
                }
            }


            [RESTRoute]
            public void HandleAllGetRequests(HttpListenerContext context)
            {
                this.SendTextResponse(context, "GET is a success!");
            }
        }
}
