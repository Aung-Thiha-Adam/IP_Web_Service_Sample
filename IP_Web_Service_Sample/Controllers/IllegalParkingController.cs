using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IP_Web_Service_Sample.Filters;
using IP_Web_Service_Sample.Utilities;
using Jose;
using IP_Web_Service_Sample.Models;

namespace IP_Web_Service_Sample.Controllers
{
    [RoutePrefix("IllegalParking")]
    public class IllegalParkingController : ApiController
    {
        const string PRIVATE_KEY_THUMBPRINT = "c07f6b76a8f42ad4f7d41203410941b74c97a7b8"; //Company A private key
        const string PUBLIC_CERT_THUMBPRINT = "3d68344f73ca59150e88cef9ca2484498e45d8d3"; //OneService public cert


        public string Index()
        {
            
            return "Hello Illegal PArking!";
        }

        // POST: api/IllegalParking
        [HttpPost]
        [Route("SubmitCase")]
        //[JwtAuthentication]
        [JwtAuthorize]
        public string SubmitCase(Case jwtToken)//([fromBody] string jwtToken)
        {
            var raw = jwtToken;
            //var decryptedJwt = Utility.DecryptPayload(raw, PRIVATE_KEY_THUMBPRINT);

            //var content = Utility.VerifyJWT(decryptedJwt, true, PUBLIC_CERT_THUMBPRINT);

            return "Congratulation: recieved request with case details:" + raw.bdy;
        }

      
    }
}
