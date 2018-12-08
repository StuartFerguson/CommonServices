using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;

namespace MessagingService.Service.Services.Email.IntegrationTest
{
    public class IntegrationTestEmailServiceProxy : IEmailServiceProxy
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)
        {
            return new SendEmailResponse
            {
                RequestId = "requestid",
                EmailId = "emailid",
                ApiStatusCode = HttpStatusCode.OK,
                Error = String.Empty,
                ErrorCode = String.Empty
            };
        }
    }
}
