using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class LibraryTest
	{
		[Test] public void CheckVersion()
		{
			string text = AntiGrain.Interface.GetVersion ();
			System.Console.Out.WriteLine ("Version: " + text);
		}
		
		[Test] public void CheckProductName()
		{
			string text = AntiGrain.Interface.GetProductName ();
			System.Console.Out.WriteLine ("ProductName: " + text);
		}
		
		[Test] public void CheckCycleDelta()
		{
			int t0 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			int t1 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			int t2 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			int t3 = Epsitec.Common.Drawing.Agg.Library.CycleDelta;
			
			System.Console.Out.WriteLine ("Deltas: " + t1.ToString () + ", " + t2.ToString () + ", " + t3.ToString ());
		}
	}
}
