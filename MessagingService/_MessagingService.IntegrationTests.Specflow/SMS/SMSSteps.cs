using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using MessagingService.IntegrationTests.Specflow.Common;
using Newtonsoft.Json;
using Shouldly;
using TechTalk.SpecFlow;

namespace MessagingService.IntegrationTests.Specflow.SMS
{
    [Binding]
    [Scope(Tag="sms")]
    public class SMSSteps : GenericSteps
    {
        public SMSSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        [Given(@"the messaging service is running")]
        public void GivenTheMessagingServiceIsRunning()
        {
            RunSystem(this.ScenarioContext.ScenarioInfo.Title);
        }
        
        [AfterScenario()]
        public void AfterScenario()
        {
            StopSystem();
        }
        
        [Given(@"My from mobile number '(.*)'")]
        public void GivenMyFromMobileNumber(String senderMobileNumber)
        {
            SendSMSRequest smsRequest = new SendSMSRequest();
            smsRequest.Sender = senderMobileNumber;
            this.ScenarioContext["SendSMSRequest"] = smsRequest;
        }
        
        [Given(@"I want to send an sms to '(.*)'")]
        public void GivenIWantToSendAnSmsTo(String destinationMobileNumber)
        {
            var smsRequest = this.ScenarioContext.Get<SendSMSRequest>("SendSMSRequest");
            smsRequest.Destination = destinationMobileNumber;
            this.ScenarioContext["SendSMSRequest"] = smsRequest;
        }
        
        [Given(@"I have the message '(.*)'")]
        public void GivenIHaveTheMessage(String message)
        {
            var smsRequest = this.ScenarioContext.Get<SendSMSRequest>("SendSMSRequest");
            smsRequest.Message = message;
            this.ScenarioContext["SendSMSRequest"] = smsRequest;
        }
        
        [When(@"I send the message")]
        public async Task WhenISendTheMessage()
        {
            var smsRequest = this.ScenarioContext.Get<SendSMSRequest>("SendSMSRequest");
            
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.MessagingServicePort}");

                String requestSerialised = JsonConvert.SerializeObject(smsRequest);
                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

                this.ScenarioContext["SendSMSHttpResponse"] = await client.PostAsync("/api/SMS", httpContent, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"the result should indicate the message was sent")]
        public async Task ThenTheResultShouldIndicateTheMessageWasSent()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("SendSMSHttpResponse");

            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var emailResponse =
                JsonConvert.DeserializeObject<SendSMSResponse>(await httpResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));

            emailResponse.ApiStatusCode.ShouldBe(HttpStatusCode.OK);
            emailResponse.MessageId.ShouldNotBeNullOrEmpty();
            emailResponse.Status.ShouldNotBeNullOrEmpty();
        }
    }
}
