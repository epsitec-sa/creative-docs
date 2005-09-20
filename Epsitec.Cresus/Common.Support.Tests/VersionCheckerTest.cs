using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class VersionCheckerTest
	{
		[Test] public void CheckVersion()
		{
			VersionChecker checker = new VersionChecker (typeof (VersionCheckerTest).Assembly);
			
			System.Console.Out.Write ("StartCheck");
			
			//	Pour tester si une version plus récente de CreativeDocs existe, il faudrait
			//	utiliser l'URL de base suivante :
			//
			//	"http://www.creativedocs.net/update/check?software=CreativeDocs&version="
			
			string url = "http://www.creativedocs.net/update/check?software=Support.Tests&version=";
			
			checker.StartCheck (url + checker.CurrentVersion);
			
			Assert.IsFalse (checker.IsReady);
			Assert.IsFalse (checker.IsCheckSuccessful);
			
			//	Attend que la réponse nous soit parvenue :
			
			while (checker.IsReady == false)
			{
				System.Console.Out.Write (".");
				System.Threading.Thread.Sleep (100);
			}
			
			System.Console.Out.WriteLine ();
			
			Assert.IsTrue (checker.IsReady);
			Assert.IsTrue (checker.IsCheckSuccessful);
			
			System.Console.Out.WriteLine ("Current version: {0}", checker.CurrentVersion);
			System.Console.Out.WriteLine ("Newer version:   {0}, {1}", checker.NewerVersion, checker.FoundNewerVersion ? "newer" : "not newer");
			System.Console.Out.WriteLine ("URL:             {0}", checker.NewerVersionUrl);
		}
	}
}
