﻿using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.SMS.TheSmsWorks
{
    public partial class TheSmsWorksExtendedErrorModelFieldError
    {
        [JsonProperty("code")]
        public String Code { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [JsonProperty("params")]
        public String[] Params { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [JsonProperty("path")]
        public String[] Path { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("description")]
        public String Description { get; set; }
    }
}