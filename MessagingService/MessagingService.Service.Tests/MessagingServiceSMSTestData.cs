using System;
using MessagingService.DataTransferObjects;
using MessagingService.Service.Commands;

namespace MessagingService.Service.Tests
{
    public class MessagingServiceSMSTestData
    {
        public static String Sender = "07777777777";
        public static String Destination = "07888888888";
        public static String Message = "Test Message";

        public static SendSMSRequest GetSendSmsRequest()
        {
            return new SendSMSRequest
            {
                Destination = Destination,
                Message = Message,
                Sender = Sender
            };
        }

        public static SendSMSCommand GetSendSmsCommand()
        {
            return SendSMSCommand.Create(GetSendSmsRequest());
        }
    }
}