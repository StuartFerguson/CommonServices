using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.DataTransferObjects;
using MessagingService.Service.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.CommandHandling;
using Shared.General;

namespace MessagingService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The command router
        /// </summary>
        private readonly ICommandRouter CommandRouter;
        
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="commandRouter">The command router.</param>
        public EmailController(ICommandRouter commandRouter)
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
        [ProducesResponseType(typeof(SendEmailResponse), 200)]
        public async Task<IActionResult> PostEmail([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation(JsonConvert.SerializeObject(request));

            SendEmailCommand command = SendEmailCommand.Create(request);

            await this.CommandRouter.Route(command, cancellationToken);

            return this.Ok(command.Response);
        }
        #endregion
    }
}