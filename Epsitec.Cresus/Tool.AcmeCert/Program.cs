//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Certes;
using Certes.Acme;
using Certes.Pkcs;
using System.Linq;
using System.Threading.Tasks;

namespace Epsitec.AcmeCert
{
    class Program
    {
        //  NuGet Package Certes > see https://github.com/fszlin/certes
        //  Example https://github.com/NiknakSi/Certiply/blob/master/Certiply/CertesWrapper.cs

        static CertificationRequestBuilder CreateCsrBuilder(CsrInfo info, string csrPem = default)
        {
            var name = $"C={info.CountryName}, ST={info.State}, L={info.Locality}, O={info.Organization}, OU={info.OrganizationUnit}, CN={info.CommonName}";
            var csr = default (CertificationRequestBuilder);

            if (string.IsNullOrEmpty (csrPem))
            {
                csr = new CertificationRequestBuilder ();
            }
            else
            {
                csr = new CertificationRequestBuilder (KeyFactory.FromPem (csrPem));
            }

            csr.AddName (name);

            return csr;
        }

        static async Task Main()
        {
            System.IO.Directory.CreateDirectory (@"C:\aider");
            System.IO.Directory.CreateDirectory (@"C:\aider\server");
            System.IO.Directory.CreateDirectory (@"C:\aider\client");
            System.IO.Directory.CreateDirectory (@"C:\aider\client\.well-known");
            System.IO.Directory.CreateDirectory (@"C:\aider\client\.well-known\acme-challenge");

            var acmeChallengeDir = @"C:\aider\client\.well-known\acme-challenge";

            System.IO.Directory.SetCurrentDirectory (@"C:\aider\server\certificate");

            var csrInfo = new CsrInfo
            {
                CountryName = "CH",
                State = "Vaud",
                Locality = "Lausanne",
                Organization = "EERV",
                OrganizationUnit = "Cedres",
                CommonName = "test-aider.eerv.ch",
            };

            var csrPem = default (string);

            if (System.IO.File.Exists ("privatekey.pem"))
            {
                csrPem = System.IO.File.ReadAllText ("privatekey.pem");
            }

            var csrBuilder = Program.CreateCsrBuilder (csrInfo, csrPem);

            var csr    = csrBuilder.Generate ();
            var csrKey = csrBuilder.Key.ToPem ();

            System.IO.File.WriteAllText ("privatekey.pem", csrKey);

            try
            {
                var server  = WellKnownServers.LetsEncryptV2;
                var acme    = default (AcmeContext);
                var account = default (IAccountContext);

                if (System.IO.File.Exists ("account.pem"))
                {
                    var accountKey = KeyFactory.FromPem (System.IO.File.ReadAllText ("account.pem"));

                    acme    = new AcmeContext (server, accountKey);
                    account = await acme.Account ();
                }
                else
                {
                    acme    = new AcmeContext (server);
                    account = await acme.NewAccount ("arnaud@epsitec.ch", true);

                    System.IO.File.WriteAllText ("account.pem", acme.AccountKey.ToPem ());
                }

                var accountInfo = await account.Resource ();

                var domains  = new string[] { csrInfo.CommonName };
                var order    = await acme.NewOrder (domains);
                var authz    = (await order.Authorizations ()).First ();
                var httpChallenge = await authz.Http ();
                var keyAuthz = httpChallenge.KeyAuthz;
                var token    = httpChallenge.Token;

                System.Console.WriteLine ($"Challenge {token}");
                System.IO.File.WriteAllText (System.IO.Path.Combine (acmeChallengeDir, token), keyAuthz);

                System.Console.WriteLine ($"Written to {acmeChallengeDir}");

                var httpChallengeResult = await httpChallenge.Validate ();

                var orderInfo = await order.Finalize (csr);

                System.Console.WriteLine ($"Status = {orderInfo.Status}");

                var certChain = await order.Download ();

                var cert = certChain.Certificate.ToPem ();
                var issuers = string.Join ("\r\n", certChain.Issuers.Select (x => x.ToPem ()));

                System.IO.File.WriteAllText ("encrypt.key", csrKey);
                System.IO.File.WriteAllText ("encrypt.pem", cert);
                System.IO.File.WriteAllText ("issuers.pem", issuers);
            }
            catch
            {
                System.Console.ReadLine ();
            }
        }
    }
}
