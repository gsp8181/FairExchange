using System;
using FEClient.API.Events;

namespace FEClient.API
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed partial class ClientRestApi
    {
        public static event EventHandler<FileSendEventArgs> FileRecieved;
        public static event EventHandler<KeyReceivedEventArgs> KeyRecieved;
        public static event EventHandler<FileSendEventArgs> FileRecievedAndRespSent;
        public static event EventHandler<NotifyRequestEventArgs> NotifyRecieved;
        public static event EventHandler<StartTransmissionEventArgs> StartTransmission;
        public static event EventHandler<FinishEventArgs> Finish;
        public static event EventHandler<StartTransmissionEventArgs> StartTransmissionAndRespSent;
    }
}