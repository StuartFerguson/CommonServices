using MessagingService.Service.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MessagingService.Service.Commands;
using Moq;
using Xunit;
using MessagingService.Service.Services;
using MessagingService.Service.Services.Email;
using MessagingService.Service.Services.SMS;
using Shouldly;

namespace MessagingService.Service.Tests
{
    public class MessagingCommandHandlerTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MessagingCommandHandler_HandleCommand_SendEmailCommand_CommandHandled(Boolean isHtml)
        {
            Mock<IEmailServiceProxy> emailServiceProxy =new Mock<IEmailServiceProxy>();
            Mock<ISMSServiceProxy> smsServiceProxy =new Mock<ISMSServiceProxy>();

            MessagingCommandHandler handler = new MessagingCommandHandler(emailServiceProxy.Object, smsServiceProxy.Object);

            SendEmailCommand command = MessagingServiceEmailTestData.GetSendEmailCommand(isHtml);

            Should.NotThrow(async () => { await handler.Handle(command, CancellationToken.None); });
        }

        [Fact]
        public void MessagingCommandHandler_HandleCommand_SendSMSCommand_CommandHandled()
        {
            Mock<IEmailServiceProxy> emailServiceProxy =new Mock<IEmailServiceProxy>();
            Mock<ISMSServiceProxy> smsServiceProxy =new Mock<ISMSServiceProxy>();

            MessagingCommandHandler handler = new MessagingCommandHandler(emailServiceProxy.Object, smsServiceProxy.Object);

            SendSMSCommand command = MessagingServiceSMSTestData.GetSendSmsCommand();

            Should.NotThrow(async () => { await handler.Handle(command, CancellationToken.None); });
        }
    }
}
