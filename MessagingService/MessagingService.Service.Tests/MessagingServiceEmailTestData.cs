using System;
using System.Collections.Generic;
using System.Text;
using MessagingService.DataTransferObjects;
using MessagingService.Service.Commands;

namespace MessagingService.Service.Tests
{
    public class MessagingServiceEmailTestData
    {
        public static String Body = "TestEmailBody";
        public static String FromAddress = "from@testaddress.com";
        public static String ToAddress = "to@testaddress.com";
        public static String Subject = "Test Email Subject";

        public static SendEmailRequest GetSendEmailRequest(Boolean isHtml)
        {
            return new SendEmailRequest
            {
                Body = Body,
                FromAddress =  FromAddress,
                ToAddresses = new List<String> { ToAddress },
                IsHtml = isHtml,
                Subject = Subject
            };
        }

        public static SendEmailCommand GetSendEmailCommand(Boolean isHtml)
        {
            return SendEmailCommand.Create(GetSendEmailRequest(isHtml));
        }               
    }
}
