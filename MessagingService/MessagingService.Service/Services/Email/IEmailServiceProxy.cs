using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;

namespace MessagingService.Service.Services.Email
{
    public interface IEmailServiceProxy
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken);
    }
}
