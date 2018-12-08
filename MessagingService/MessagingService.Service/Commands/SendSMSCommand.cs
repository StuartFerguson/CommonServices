using System;
using MessagingService.DataTransferObjects;
using Shared.CommandHandling;

namespace MessagingService.Service.Commands
{
    public class SendSMSCommand : Command<SendSMSResponse>
    {
        #region Properties

        /// <summary>
        /// Gets the send SMS request.
        /// </summary>
        /// <value>
        /// The send SMS request.
        /// </value>
        public SendSMSRequest SendSMSRequest { get; private set; }

        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSMSCommand" /> class.
        /// </summary>
        /// <param name="sendSmsRequest">The send SMS request.</param>
        /// <param name="commandId">The command identifier.</param>
        private SendSMSCommand(SendSMSRequest sendSmsRequest, Guid commandId) : base(commandId)
        {
            this.SendSMSRequest= sendSmsRequest;
        }
        #endregion

        #region public static SendSMSCommand Create(SendSMSRequest sendSMSRequest)                
        /// <summary>
        /// Creates the specified send SMS request.
        /// </summary>
        /// <param name="sendSmsRequest">The send SMS request.</param>
        /// <returns></returns>
        public static SendSMSCommand Create(SendSMSRequest sendSmsRequest)
        {
            return new SendSMSCommand(sendSmsRequest, Guid.NewGuid());
        }
        #endregion
    }
}