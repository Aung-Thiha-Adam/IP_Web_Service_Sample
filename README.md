# IP_Web_Service_Sample
IP_Web_Service_Sample



Signing the jwt

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
            


Encrypting the JWT

            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)GetCertificate(publickeyThumbprint).PublicKey.Key;

            return JWT.Encode(payload, // content as String read from Response
                    publicKey, // RSACryptoServiceProvider
                   JweAlgorithm.RSA_OAEP,
                   JweEncryption.A256GCM).Trim(new char[] { '"' });


Decrypting the payload

            RSACryptoServiceProvider privateKeyToDecrypt = new RSACryptoServiceProvider();
            privateKeyToDecrypt.FromXmlString(Utility.GetCertificate(privateKeyThumbprint).PrivateKey.ToXmlString(true));

            return JWT.Decode(encryptedPayload, // content as String read from Response
                    privateKeyToDecrypt, // RSACryptoServiceProvider
                    JweAlgorithm.RSA_OAEP,
                    JweEncryption.A256GCM).Trim(new char[] { '"' });
                    

Verifying the signature

            RSACryptoServiceProvider key = (RSACryptoServiceProvider)Utility.GetCertificate(publicCertThumbprint).PublicKey.Key;

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(key);
            rsaDeformatter.SetHashAlgorithm("SHA256");
            if (!rsaDeformatter.VerifySignature(hash, FromBase64Url(parts[2])))
            {
                throw new SecurityException("Invalid Signature");
            }
