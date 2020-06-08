/// <summary>
/// author: Stephan van Rooij, Peter Polačko, Merijn Hendriks
/// license: CC BY-NC-SA 4.0
/// sources:
/// https://svrooij.nl/2018/04/generate-x509certificate2-in-csharp/
/// https://stackoverflow.com/a/52102405
/// https://stackoverflow.com/a/41720563
/// </summary>

using System;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using EmuTarkov.Common.Utils.App;
using EmuTarkov.Server.Utils.App;

namespace EmuTarkov.Server.Utils.Server
{
    public class SelfSigned
    {
        public SelfSigned(string subject, int port)
        {
            // generate certifcate if it doesn't exist
            if (Load(subject) == null)
            {
                X509Certificate2 cert = Generate(subject);
                Save(cert);

                var appid = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>().Value.ToUpper();
                CommandPrompt.ExecuteCommand($"netsh http add urlacl url=https://+:{port}/ user=everyone");
                CommandPrompt.ExecuteCommand($"netsh http add sslcert ipport=0.0.0.0:{port} certstorename=My certhash={cert.Thumbprint} appid={{{appid}}}");
            }
        }

        /// <summary>
        /// Generate a self-signed certificate
        /// </summary>
        public X509Certificate2 Generate(string subject)
        {
            SecureRandom random = new SecureRandom();

            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random));
            certificateGenerator.SetIssuerDN(new X509Name($"CN={subject}"));
            certificateGenerator.SetSubjectDN(new X509Name($"CN={subject}"));
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(1));

            const int strength = 2048;
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, strength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);

            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;
            const string signatureAlgorithm = "SHA256WithRSA";
            Asn1SignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private);
            Org.BouncyCastle.X509.X509Certificate bouncyCert = certificateGenerator.Generate(signatureFactory);

            // Convert it to X509Certificate2
            X509Certificate2 certificate;

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry($"{subject}_key", new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { new X509CertificateEntry(bouncyCert) });
            string exportpw = Guid.NewGuid().ToString("x");

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                store.Save(ms, exportpw.ToCharArray(), random);
                certificate = new X509Certificate2(ms.ToArray(), exportpw, X509KeyStorageFlags.Exportable);
            }

            Logger.LogInfo($"EmuTarkov.Server: Generated cert with thumbprint {certificate.Thumbprint}");
            return certificate;
        }

        /// <summary>
        /// Load certificate from local store
        /// </summary>
        private X509Certificate2 Load(string subject)
        {
            try
            {
                X509Store userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                userStore.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = userStore.Certificates.Find(X509FindType.FindBySubjectName, Environment.MachineName, false);

                if (collection.Count > 0)
                {
                    return collection[0];
                }

                userStore.Close();
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Save certificate to local store
        /// </summary>
        private void Save(X509Certificate2 certificate)
        {
            try
            {
                X509Store userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                userStore.Open(OpenFlags.ReadWrite);
                userStore.Add(certificate);
                userStore.Close();
            }
            catch { }
        }
    }
}

