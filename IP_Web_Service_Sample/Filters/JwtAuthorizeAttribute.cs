using IP_Web_Service_Sample.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Security;
using System.Net;
using System.Security.Cryptography;


namespace IP_Web_Service_Sample.Filters
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var requestbody = actionContext.Request.Content.ReadAsStringAsync().Result;
                requestbody = requestbody.Trim('"');
                var decryptedJwt = Utility.DecryptPayload(requestbody, Utility.PRIVATE_KEY_THUMBPRINT);

                var content = Utility.VerifyJWT(decryptedJwt, true, Utility.PUBLIC_CERT_THUMBPRINT);
                //content = string.Format("{0}{1}{0}", "\"", content);
                actionContext.Request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            catch(Exception ex)
            {
                if (ex is SecurityException || ex is CryptographicException)
                    actionContext.Response = new HttpResponseMessage((HttpStatusCode)401) { ReasonPhrase = "Unauthorized" };
                else
                    actionContext.Response = new HttpResponseMessage((HttpStatusCode)500) { ReasonPhrase = "Internal Error" };
            }
                
        }
    }
}