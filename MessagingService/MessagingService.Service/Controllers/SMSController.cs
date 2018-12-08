using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using MessagingService.Service.Commands;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.CommandHandling;
using Shared.General;

namespace MessagingService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The command router
        /// </summary>
        private readonly ICommandRouter CommandRouter;
        
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="SMSController"/> class.
        /// </summary>
        /// <param name="commandRouter">The command router.</param>
        public SMSController(ICommandRouter commandRouter)
        {
            this.CommandRouter = commandRouter;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Posts the email.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SendSMSResponse), 200)]
        public async Task<IActionResult> PostSMS([FromBody] SendSMSRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation(JsonConvert.SerializeObject(request));

            SendSMSCommand command = SendSMSCommand.Create(request);

            await this.CommandRouter.Route(command, cancellationToken);

            return this.Ok(command.Response);
        }
        #endregion
    }
}