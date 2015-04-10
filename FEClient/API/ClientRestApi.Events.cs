namespace FEClient.API
{
    public sealed partial class ClientRestApi
    {
        public delegate void FileRecievedAndRespSendHandler(object sender, FileSend file);

        //public delegate void NotifyHandler(object sender, string myValue);
        //public static event NotifyHandler Notify = delegate { };

        public delegate void FileRecievedHandler(object sender, FileSend file, NotifyArgs callbackArgs);

        public delegate void FinishHandler(object sender, string guid, NotifyArgs callbackArgs);

        public delegate void KeyRecievedHandler(object sender, KeyArgs key, NotifyArgs callbackArgs);

        public delegate void NotifyRecievedHandler(object sender, NotifyRequest vars);

        public delegate void StartTransmissionAndRespSentHandler(object sender, NotifyRequest vars);

        public delegate void StartTransmissionHandler(object sender, NotifyRequest vars, NotifyArgs callbackArgs);

        public static event FileRecievedHandler FileRecieved = delegate { };
        public static event KeyRecievedHandler KeyRecieved = delegate { };
        public static event FileRecievedAndRespSendHandler FileRecievedAndRespSent = delegate { };
        public static event NotifyRecievedHandler NotifyRecieved = delegate { };
        public static event StartTransmissionHandler StartTransmission = delegate { };
        public static event FinishHandler Finish = delegate { };
        public static event StartTransmissionAndRespSentHandler StartTransmissionAndRespSent = delegate { };
    }
}