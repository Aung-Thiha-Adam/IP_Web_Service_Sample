using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using Jose.jwe;
using IP_Web_Service_Sample.Utilities;
using Jose;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Http.Results;

namespace IP_Web_Service_Sample.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => false;

  

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var requestbody = await context.Request.Content.ReadAsStringAsync();
            var decryptedJwt = Utility.DecryptPayload(requestbody, Utility.PRIVATE_KEY_THUMBPRINT);

            var content = Utility.VerifyJWT(decryptedJwt, true, Utility.PUBLIC_CERT_THUMBPRINT);

            context.Request.Content = new StringContent(content);
           
        }
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(
                             new AuthenticationHeaderValue("Basic", "realm=localhost"));
            }
            context.Result = new ResponseMessageResult(result);
        }

    }
}