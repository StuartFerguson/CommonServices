﻿using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.SMS.TheSmsWorks
{
    public partial class TheSmsWorksExtendedErrorModel
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        [JsonProperty("errors")]
        public TheSmsWorksExtendedErrorModelError[] Errors { get; set; }
    }
}