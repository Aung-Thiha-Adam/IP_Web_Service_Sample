using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace IP_Web_Service_Sample.Utilities
{
    public static class Utility
    {
        public static string PRIVATE_KEY_THUMBPRINT = "c07f6b76a8f42ad4f7d41203410941b74c97a7b8"; //Company A private key
        public static string PUBLIC_CERT_THUMBPRINT = "3d68344f73ca59150e88cef9ca2484498e45d8d3"; //OneService public cert


        public static X509Certificate2 GetCertificate(string thumbPrint)
        {
            X509Certificate2 certSelected = null;
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.MaxAllowed);

            X509Certificate2Collection certificateCollection = x509Store.Certificates;
            foreach (X509Certificate2 cert in certificateCollection)
            {
                //string thumbPrint = CommonUtils.GetAppKey("EPSCertificateThumbPrint").ToLower();
                if (cert.Thumbprint.Replace(" ", "").ToLower().Equals(thumbPrint))
                {
                    certSelected = cert;
                    break;
                }
            }
            x509Store.Close();

            return certSelected;
        }


        public static string DecryptPayload(string encryptedPayload, string privateKeyThumbprint)
        {
            RSACryptoServiceProvider privateKeyToDecrypt = new RSACryptoServiceProvider();
            privateKeyToDecrypt.FromXmlString(Utility.GetCertificate(privateKeyThumbprint).PrivateKey.ToXmlString(true));

            return JWT.Decode(encryptedPayload, // content as String read from Response
                    privateKeyToDecrypt, // RSACryptoServiceProvider
                    JweAlgorithm.RSA_OAEP,
                    JweEncryption.A256GCM).Trim(new char[] { '"' });
        }


        public static string VerifyJWT(string jwt, bool verify, string publicCertThumbprint)
        {
            string[] parts = jwt.Split('.');
            string header = parts[0];
            string payload = parts[1];
            byte[] crypto = Encoding.UTF8.GetBytes(parts[2]);

            string payloadJson = Encoding.UTF8.GetString(FromBase64Url(payload));
            //var jsonObject = payloadJson.FromJSON();

            if (verify)
            {

                RSACryptoServiceProvider key = (RSACryptoServiceProvider)Utility.GetCertificate(publicCertThumbprint).PublicKey.Key;

                SHA256 sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(key);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                if (!rsaDeformatter.VerifySignature(hash, FromBase64Url(parts[2])))
                {
                    throw new SecurityException("Invalid Signature");
                }

                //if (!key.VerifyData(hash, new SHA256CryptoServiceProvider(), Convert.FromBase64String(parts[2])))
                //throw new Exception("Invalid signature");
            }
            return payloadJson.ToString();
        }

        public static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

       

    }
}