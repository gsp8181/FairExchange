namespace FEClient.API
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed partial class ClientRestApi
    {
        public delegate void FileRecievedAndRespSendHandler(object sender, FileSendEventArgs e);

        public delegate void FileRecievedHandler(object sender, FileSendEventArgs e);

        public delegate void FinishHandler(object sender, FinishEventArgs e);

        public delegate void KeyRecievedHandler(object sender, KeyReceivedEventArgs e);

        public delegate void NotifyRecievedHandler(object sender, NotifyRequestEventArgs e);

        public delegate void StartTransmissionAndRespSentHandler(object sender, StartTransmissionEventArgs e);

        public delegate void StartTransmissionHandler(object sender, StartTransmissionEventArgs e);

        public static event FileRecievedHandler FileRecieved = delegate { };
        public static event KeyRecievedHandler KeyRecieved = delegate { };
        public static event FileRecievedAndRespSendHandler FileRecievedAndRespSent = delegate { };
        public static event NotifyRecievedHandler NotifyRecieved = delegate { };
        public static event StartTransmissionHandler StartTransmission = delegate { };
        public static event FinishHandler Finish = delegate { };
        public static event StartTransmissionAndRespSentHandler StartTransmissionAndRespSent = delegate { };
    }
}