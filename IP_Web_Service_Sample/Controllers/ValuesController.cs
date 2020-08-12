using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IP_Web_Service_Sample.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {


        // GET api/values
        [HttpGet]
        [Route("getmethod")]
        public HttpResponseMessage Get()
        {
            return  Request.CreateResponse<string>(HttpStatusCode.OK, "testing ok");
        }

        
    }
}
