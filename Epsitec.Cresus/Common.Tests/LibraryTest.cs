using System;
using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class LibraryTest
	{
		[Test] public void CheckVersion()
		{
			string text = Epsitec.Common.Drawing.Agg.Library.Current.Version;
			System.Console.Out.WriteLine ("Version: " + text);
		}
		
		[Test] public void CheckProductName()
		{
			string text = Epsitec.Common.Drawing.Agg.Library.Current.ProductName;
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
