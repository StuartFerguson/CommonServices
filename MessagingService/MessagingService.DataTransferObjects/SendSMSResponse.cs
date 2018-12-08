using System;
using System.Net;

namespace MessagingService.DataTransferObjects
{
    public class SendSMSResponse
    {
        /// <summary>
        /// Gets or sets the API status code.
        /// </summary>
        /// <value>
        /// The API status code.
        /// </value>
        public HttpStatusCode ApiStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public String MessageId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public String Status { get; set; }
    }
}