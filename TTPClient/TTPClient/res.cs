using Grapevine;
using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient
{
        public sealed class MyResource : RESTResource
        {
            [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^/foo/\d+$")]
            [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/bar/\D+$")]
            public void HandleFooBarRequests(HttpListenerContext context)
            {
                this.SendTextResponse(context, "foo bar is a success!");
            }

            [RESTRoute]
            public void HandleAllGetRequests(HttpListenerContext context)
            {
                this.SendTextResponse(context, "GET is a success!");
            }
        }
}
