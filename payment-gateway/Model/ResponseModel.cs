using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    /// <summary>
    /// This class represents the json response model 
    /// applied in all endpoints support Json media type response
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ResponseModel
    {
        /// <summary>
        /// The response status by default is True
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; } = true;

        /// <summary>
        /// The total records in the response
        /// </summary>
        [JsonProperty("total-records")]
        public long TotalRecords { get; set; } = 1;

        /// <summary>
        /// Response data
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "data")]
        [DefaultValue(null)]
        public object Data { get; set; }

        /// <summary>
        /// The error message 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "error-message")]
        [DefaultValue("")]
        public string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        ///the error code 
        ///<remarks>find further description in TF WIKI filtering by error code.</remarks>
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "error-code")]
        [DefaultValue("")]
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// The error stack trace for debugging trace 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "error-stack-trace")]
        [DefaultValue("")]
        public string ErrorStackTrace { get; set; } = string.Empty;
    }
}
