using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// postgres table map entity class
    /// </summary>
    public class DataProtectionKey
    {
        /// <summary>
        /// friendly name
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// xml content
        /// </summary>
        public string XmlData { get; set; }
    }
}
