using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IP_Web_Service_Sample.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Case
    {
        public string iss { get; set; }
        public string aud { get; set; }
        public string bdy { get; set; }
    }
}