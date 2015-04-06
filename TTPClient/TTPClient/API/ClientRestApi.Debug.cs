using System.Net;
using FEClient.Security;
using Grapevine;
using Grapevine.Server;
using Newtonsoft.Json.Linq;

namespace FEClient.API
{
    public sealed partial class ClientRestApi
    {
#if DEBUG
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/decrypt/?$")]
        public void HandleDecryptRequest(HttpListenerContext context)
        {
            var payload = JObject.Parse(this.GetPayload(context.Request));
            var data = payload.Value<string>("data");
            var key = payload.Value<string>("key");
            context.Response.ContentType = "text/plain";
            this.SendTextResponse(context, Rsa.DecryptData(data, key));
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/encrypt/?$")]
        public void HandleEncryptRequest(HttpListenerContext context)
        {
            //JObject response = new JObject();
            //response.Add("Key",key);
            //response.Add("data", data);
            this.SendJsonResponse(context, Rsa.EncryptData(this.GetPayload(context.Request)));
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/sign/?$")]
        public void HandleSignRequest(HttpListenerContext context)
        {
            context.Response.ContentType = "text/plain";
            this.SendTextResponse(context, Rsa.SignData(this.GetPayload(context.Request)));
        }
        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/encryptTo/?$")]
        public void HandleEncryptToRequest(HttpListenerContext context)
        {
            var payload = JObject.Parse(this.GetPayload(context.Request));
            var data = payload.Value<string>("data");
            var key = payload.Value<string>("key");
            this.SendJsonResponse(context, Rsa.EncryptData(data, key));
        }
#endif
    }
}
