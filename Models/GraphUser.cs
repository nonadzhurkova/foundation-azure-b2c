using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Azure.Models
{
    /// <summary>
    ///  Graph User from json result.
    /// </summary>
    public class GraphUser
    {
        /// <summary>
        ///  Gets or sets the wrapper in json result.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }

        /// <summary>
        ///  Gets or sets list with users.
        /// </summary>
        [JsonProperty("value")]
        public List<Value> Users { get; set; }
    }

    public class Value
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        public string id { get; set; }
    }

}