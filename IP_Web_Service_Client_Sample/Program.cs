using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IP_Web_Service_Client_Sample
{
    class Program
    {

        const string PRIVATE_KEY_THUMBPRINT = "3d68344f73ca59150e88cef9ca2484498e45d8d3"; //oneservice private key
        const string PUBLIC_CERT_THUMBPRINT = "c07f6b76a8f42ad4f7d41203410941b74c97a7b8"; //Agency public cert
        const string TEST_URL = "https://localhost:44324/IllegalParking/SubmitCase";

        static void Main(string[] args)
        {

            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            try
            {
                var client = new HttpClient();

                var caseDetails = new { caseid = "12345", description = "This is sample case", submissionDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ"), isPriority = true };

                
                var requestBdy = CreateRequest(caseDetails.ToJSON());
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TEST_URL);
                Console.WriteLine(string.Format("Request content: {0}", requestBdy));
                request.Content = new StringContent(requestBdy, Encoding.UTF8, "application/json");

                Console.WriteLine(string.Format("Calling the API: {0}", TEST_URL));
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine(string.Format("Response payload: {0}",responseBody));
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        private static string CreateRequest(string bodyContent)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            var header = new { alg = "RS256", typ = "JWT" };
            var body = new { iss = "MSO", aud = "AGT", bdy = bodyContent };
            var encSignature = GenerateSignature(header.ToJSON(), body.ToJSON(), PRIVATE_KEY_THUMBPRINT);

            var encHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(header.ToJSON()));
            var encBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(body.ToJSON()));

            var jwt = string.Format("{0}.{1}.{2}", encHeader, encBody, encSignature);

            var encyptedJwt = EncryptJWT(jwt, PUBLIC_CERT_THUMBPRINT);

            return encyptedJwt;
        }


        private static string GenerateSignature(string header, string body, string privatekeyThumbprint)
        {
            var encodedHdr= Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            var encodedBdy = Convert.ToBase64String(Encoding.UTF8.GetBytes(body));

            SHA256 sha256 = SHA256.Create();
            byte[] originalData = sha256.ComputeHash(Encoding.UTF8.GetBytes(encodedHdr + '.' + encodedBdy));

            
            byte[] signedData;

            RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider();
            privateKey.FromXmlString(GetCertificate(privatekeyThumbprint).PrivateKey.ToXmlString(true));

            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(privateKey);
            RSAFormatter.SetHashAlgorithm("SHA256");
            signedData = RSAFormatter.CreateSignature(originalData);

            return Convert.ToBase64String(signedData);
        }

        private static string EncryptJWT(string payload, string publickeyThumbprint)
        {
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)GetCertificate(publickeyThumbprint).PublicKey.Key;

            return JWT.Encode(payload, // content as String read from Response
                    publicKey, // RSACryptoServiceProvider
                   JweAlgorithm.RSA_OAEP,
                   JweEncryption.A256GCM).Trim(new char[] { '"' });
        }

        private static X509Certificate2 GetCertificate(string thumbPrint)
        {
            X509Certificate2 certSelected = null;
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.MaxAllowed);

            X509Certificate2Collection certificateCollection = x509Store.Certificates;
            foreach (X509Certificate2 cert in certificateCollection)
            {
                if (cert.Thumbprint.Replace(" ", "").ToLower().Equals(thumbPrint))
                {
                    certSelected = cert;
                    break;
                }
            }
            x509Store.Close();

            return certSelected;
        }

    }
}
