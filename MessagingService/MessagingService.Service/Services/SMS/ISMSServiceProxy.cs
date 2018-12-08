using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.SMS
{
    public interface ISMSServiceProxy
    {
        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SendSMSResponse> SendSMS(SendSMSRequest request, CancellationToken cancellationToken);
    }
}
