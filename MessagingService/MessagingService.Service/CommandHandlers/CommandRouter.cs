using System.Threading;
using System.Threading.Tasks;
using MessagingService.Service.Commands;
using MessagingService.Service.Services;
using MessagingService.Service.Services.Email;
using MessagingService.Service.Services.SMS;
using Shared.CommandHandling;

namespace MessagingService.Service.CommandHandlers
{
    public class CommandRouter : ICommandRouter
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

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRouter" /> class.
        /// </summary>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        /// <param name="smsServiceProxy">The SMS service proxy.</param>
        public CommandRouter(IEmailServiceProxy emailServiceProxy, ISMSServiceProxy smsServiceProxy)
        {
            this.EmailServiceProxy = emailServiceProxy;
            this.SMSServiceProxy = smsServiceProxy;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Routes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task Route(ICommand command, CancellationToken cancellationToken)
        {
            ICommandHandler commandHandler = CreateHandler((dynamic)command);

            await commandHandler.Handle(command, cancellationToken);
        }

        #endregion

        #region Private Methods

        #region private ICommandHandler CreateHandler(CreateClubConfigurationCommand command)        
        /// <summary>
        /// Creates the handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private ICommandHandler CreateHandler(SendSMSCommand command)
        {
            return new MessagingCommandHandler(this.EmailServiceProxy, this.SMSServiceProxy);
        }
        #endregion

        #region private ICommandHandler CreateHandler(CreateClubConfigurationCommand command)        
        /// <summary>
        /// Creates the handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private ICommandHandler CreateHandler(SendEmailCommand command)
        {
            return new MessagingCommandHandler(this.EmailServiceProxy, this.SMSServiceProxy);
        }
        #endregion

        #endregion
    }
}