using System.Threading;
using System.Threading.Tasks;
using MessagingService.Service.Commands;
using MessagingService.Service.Services;
using MessagingService.Service.Services.Email;
using MessagingService.Service.Services.SMS;
using Shared.CommandHandling;

namespace MessagingService.Service.CommandHandlers
{
    public class MessagingCommandHandler : ICommandHandler
    {
        #region Fields

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        /// <summary>
        /// The SMS service proxy
        /// </summary>
        private readonly ISMSServiceProxy SMSServiceProxy;

        #endregion

        #region Constructor            
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandHandler" /> class.
        /// </summary>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        /// <param name="smsServiceProxy">The SMS service proxy.</param>
        public MessagingCommandHandler(IEmailServiceProxy emailServiceProxy, ISMSServiceProxy smsServiceProxy)
        {
            this.EmailServiceProxy = emailServiceProxy;
            this.SMSServiceProxy = smsServiceProxy;
        }
        #endregion

        #region Public Methods        
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task Handle(ICommand command, CancellationToken cancellationToken)
        {
            await this.HandleCommand((dynamic)command, cancellationToken);
        }
        #endregion

        #region Private Methods

        #region private async Task HandleCommand(SendEmailCommand command, CancellationToken cancellationToken)        
        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task HandleCommand(SendEmailCommand command, CancellationToken cancellationToken)
        {
            var response = await this.EmailServiceProxy.SendEmail(command.SendEmailRequest, cancellationToken);

            command.Response = response;
        }
        #endregion

        #region private async Task HandleCommand(SendSMSCommand command, CancellationToken cancellationToken)
        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task HandleCommand(SendSMSCommand command, CancellationToken cancellationToken)
        {
            var response = await this.SMSServiceProxy.SendSMS(command.SendSMSRequest, cancellationToken);

            command.Response = response;
        }
        #endregion

        #endregion
    }
}