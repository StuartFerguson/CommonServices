using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;

namespace MessagingService.Service.Services.SMS.IntegrationTest
{
    public class IntegrationTestSMSServiceProxy : ISMSServiceProxy
    {
        public async Task<SendSMSResponse> SendSMS(SendSMSRequest request, CancellationToken cancellationToken)
        {
            return new SendSMSResponse
            {
                ApiStatusCode = HttpStatusCode.OK,
                MessageId = "messageid",
                Status = "status"
            };
        }
    }
}