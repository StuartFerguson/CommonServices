using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using Newtonsoft.Json;
using Shared.General;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    public class Smtp2GoProxy : IEmailServiceProxy
    {
        #region Public Methods

        #region public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)
        {
            SendEmailResponse response = null;

            // Translate the request message
            var apiRequest = new Smtp2GoSendEmailRequest
            {
                ApiKey = ConfigurationReader.GetValue("SMTP2GoAPIKey"),
                HTMLBody = request.IsHtml ? request.Body : String.Empty,
                TextBody = request.IsHtml ? String.Empty : request.Body,
                Sender = request.FromAddress,
                Subject = request.Subject,
                TestMode = false,
                To = request.ToAddresses.ToArray()
            };

            String requestSerialised = JsonConvert.SerializeObject(apiRequest);
            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationReader.GetValue("SMTP2GoBaseAddress"));

                var httpResponse = await client.PostAsync("email/send", content, cancellationToken);

                var apiResponse =
                    JsonConvert.DeserializeObject<Smtp2GoSendEmailResponse>(await httpResponse.Content.ReadAsStringAsync());

                // Translate the Response
                response = new SendEmailResponse
                {
                    ApiStatusCode = httpResponse.StatusCode,
                    EmailId = apiResponse.Data.EmailId,
                    Error = apiResponse.Data.Error,
                    ErrorCode = apiResponse.Data.ErrorCode,
                    RequestId = apiResponse.RequestId
                };
            }
            
            return response;
        }
        #endregion

        #endregion
    }
}