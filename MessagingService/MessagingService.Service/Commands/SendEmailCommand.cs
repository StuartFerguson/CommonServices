using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MessagingService.DataTransferObjects;
using Shared.CommandHandling;

namespace MessagingService.Service.Commands
{
    public class SendEmailCommand : Command<SendEmailResponse>
    {
        #region Properties

        /// <summary>
        /// Gets the send email request.
        /// </summary>
        /// <value>
        /// The send email request.
        /// </value>
        public SendEmailRequest SendEmailRequest { get; private set; }

        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailCommand"/> class.
        /// </summary>
        /// <param name="sendEmailRequest">The send email request.</param>
        /// <param name="commandId">The command identifier.</param>
        private SendEmailCommand(SendEmailRequest sendEmailRequest, Guid commandId) : base(commandId)
        {
            this.SendEmailRequest = sendEmailRequest;
        }
        #endregion

        #region public static SendEmailCommand Create(SendEmailRequest sendEmailRequest)        
        /// <summary>
        /// Creates the specified send email request.
        /// </summary>
        /// <param name="sendEmailRequest">The send email request.</param>
        /// <returns></returns>
        public static SendEmailCommand Create(SendEmailRequest sendEmailRequest)
        {
            return new SendEmailCommand(sendEmailRequest, Guid.NewGuid());
        }
        #endregion
    }
}
