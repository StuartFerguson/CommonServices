using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using Newtonsoft.Json;
using Shared.Extensions;
using Shared.General;

namespace MessagingService.Service.Services.SMS.TheSmsWorks
{
    public class TheSmsWorksProxy : ISMSServiceProxy
    {
        #region Public Methods

        #region public async Task<SendSMSResponse> SendSMS(SendSMSRequest request, CancellationToken cancellationToken)        
        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendSMSResponse> SendSMS(SendSMSRequest request, CancellationToken cancellationToken)
        {
            SendSMSResponse response = null;

            using (HttpClient client = new HttpClient())
            {
                // Create the Auth Request                
                TheSmsWorksTokenRequest apiTokenRequest = new TheSmsWorksTokenRequest
                {
                    CustomerId = ConfigurationReader.GetValue("TheSMSWorksCustomerId"),
                    Key = ConfigurationReader.GetValue("TheSMSWorksKey"),
                    Secret = ConfigurationReader.GetValue("TheSMSWorksSecret")
                };

                String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
                StringContent content = new StringContent(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

                // First do the authentication
                var apiTokenHttpResponse = await client.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}auth/token", content, cancellationToken);

                if (apiTokenHttpResponse.IsSuccessStatusCode)
                {
                    TheSmsWorksTokenResponse apiTokenResponse =
                        JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content
                            .ReadAsStringAsync());

                    // Now do the actual send
                    TheSmsWorksSendSMSRequest apiSendSmsRequest = new TheSmsWorksSendSMSRequest
                    {
                        Content = request.Message,
                        Sender = request.Sender,
                        Destination = request.Destination,
                        Schedule = string.Empty,
                        Tag = string.Empty,
                        Ttl = 0
                    };

                    String apiSendSMSMessageRequestSerialised = JsonConvert.SerializeObject(apiSendSmsRequest).ToLower();
                    content = new StringContent(apiSendSMSMessageRequestSerialised, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Authorization", apiTokenResponse.Token);
                    var apiSendSMSMessageHttpResponse = await client.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}message/send", content, cancellationToken);

                    if (apiSendSMSMessageHttpResponse.IsSuccessStatusCode)
                    {
                        // Message has been sent
                        TheSmsWorksSendSMSResponse apiTheSmsWorksSendSmsResponse =
                            JsonConvert.DeserializeObject<TheSmsWorksSendSMSResponse>(await apiSendSMSMessageHttpResponse.Content
                                .ReadAsStringAsync());

                        response = new SendSMSResponse
                        {
                            ApiStatusCode = apiSendSMSMessageHttpResponse.StatusCode,
                            MessageId = apiTheSmsWorksSendSmsResponse.MessageId,
                            Status = apiTheSmsWorksSendSmsResponse.Status
                        };
                    }
                    else
                    {
                        response = await HandleAPIError(apiSendSMSMessageHttpResponse);
                    }
                }
                else
                {
                    response = await HandleAPIError(apiTokenHttpResponse);
                }

            }

            return response;
        }
        #endregion

        #endregion
        
        #region Private Methods

        #region private async Task<SendSMSResponse> HandleAPIError(HttpResponseMessage httpResponse)        
        /// <summary>
        /// Handles the API error.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <returns></returns>
        private async Task<SendSMSResponse> HandleAPIError(HttpResponseMessage httpResponse)
        {
            SendSMSResponse response;

            String responseContent = await httpResponse.Content.ReadAsStringAsync();

            var isValidObject = responseContent.TryParseJson(out TheSmsWorksExtendedErrorModel errorModel);

            if (isValidObject)
            {
                response = new SendSMSResponse
                {
                    ApiStatusCode = httpResponse.StatusCode,
                    MessageId = errorModel.Message,
                    Status = null
                };
            }
            else
            {
                response = new SendSMSResponse
                {
                    ApiStatusCode = httpResponse.StatusCode,
                    MessageId = responseContent,
                    Status = null
                };
            }

            return response;
        }
        #endregion

        #endregion
    }
}