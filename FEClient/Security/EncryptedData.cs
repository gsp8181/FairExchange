using Newtonsoft.Json.Linq;

namespace FEClient.Security
{
    public class EncryptedData
    {
        public string Data { get; set; }
        public string Key { get; set; }

        public override string ToString()
        {
            return ToJObject().ToString();
        }

        public JObject ToJObject()
        {
            var output = new JObject {{"key", Key}, {"data", Data}};
            return output;
        }
    }
}