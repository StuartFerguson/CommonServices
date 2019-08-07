using System;
using System.Collections.Generic;
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

namespace MessagingService.IntegrationTests.Specflow.Email
{
    [Binding]
    [Scope(Tag="email")]
    public class EmailSteps : GenericSteps
    {
        public EmailSteps(ScenarioContext scenarioContext) : base(scenarioContext)
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

        [Given(@"My from email address is '(.*)'")]
        public void GivenMyFromEmailAddressIs(String fromAddress)
        {
            SendEmailRequest emailRequest = new SendEmailRequest();
            emailRequest.FromAddress = fromAddress;
            this.ScenarioContext["SendEmailRequest"] = emailRequest;
        }
        
        [Given(@"I want to send an email to '(.*)'")]
        public void GivenIWantToSendAnEmailTo(String toAddress)
        {
            var emailRequest = this.ScenarioContext.Get<SendEmailRequest>("SendEmailRequest");
            emailRequest.ToAddresses = new List<String> {toAddress};
            this.ScenarioContext["SendEmailRequest"] = emailRequest;
        }
        
        [Given(@"I have the message '(.*)'")]
        public void GivenIHaveTheMessage(String message)
        {
            var emailRequest = this.ScenarioContext.Get<SendEmailRequest>("SendEmailRequest");
            emailRequest.Body = message;
            this.ScenarioContext["SendEmailRequest"] = emailRequest;
        }
        
        [Given(@"I have a message subject '(.*)'")]
        public void GivenIHaveAMessageSubject(String subject)
        {
            var emailRequest = this.ScenarioContext.Get<SendEmailRequest>("SendEmailRequest");
            emailRequest.Subject = subject;
            this.ScenarioContext["SendEmailRequest"] = emailRequest;
        }


        [When(@"I send the message")]
        public async Task WhenISendTheMessage()
        {
            var emailRequest = this.ScenarioContext.Get<SendEmailRequest>("SendEmailRequest");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.MessagingServicePort}");

                String requestSerialised = JsonConvert.SerializeObject(emailRequest);
                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

                this.ScenarioContext["SendEmailHttpResponse"] = await client.PostAsync("/api/Email", httpContent, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"the result should indicate the message was sent")]
        public async Task ThenTheResultShouldIndicateTheMessageWasSent()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("SendEmailHttpResponse");

            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var emailResponse =
                JsonConvert.DeserializeObject<SendEmailResponse>(await httpResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));

            emailResponse.ApiStatusCode.ShouldBe(HttpStatusCode.OK);
            emailResponse.EmailId.ShouldNotBeNullOrEmpty();
            emailResponse.RequestId.ShouldNotBeNullOrEmpty();
        }
    }
}
