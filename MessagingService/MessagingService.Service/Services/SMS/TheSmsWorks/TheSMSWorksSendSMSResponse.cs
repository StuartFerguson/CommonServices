using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.SMS.TheSmsWorks
{
    public class TheSmsWorksSendSMSResponse
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonProperty("messageid")]
        public String MessageId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        /// <value>
        /// The credits.
        /// </value>
        [JsonProperty("credits")]
        public Int32 Credits { get; set; }
    }
}