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
	}
}
