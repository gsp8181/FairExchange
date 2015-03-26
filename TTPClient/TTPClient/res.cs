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
                var input = context.Request.InputStream;
                StreamReader sr = new StreamReader(input);
                String x = sr.ReadToEnd();
                this.SendTextResponse(context, x);
                FileRecieved(this, x);
            }

            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/notify/$")]
            public void HandleNotifyRecievedRequest(HttpListenerContext context)
            {
                //NotifyRecieved(this, );
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
