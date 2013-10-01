using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{

	[TestClass]
	public class DteTest
	{
		[TestMethod]
		public void TestMethod1()
		{
			var dte = DTE2Provider.EnumDTE2s ().Where(dte2 => dte2.Solution != null && string.Compare (dte2.Solution.FullName, Path.GetFullPath(TestData.CresusGraphSolutionPath), true) == 0).First ();

			var events = dte.Watch ();

			using (events.Subscribe (n => Trace.WriteLine (n)))
			{
				Thread.Sleep (1000);
			}
		}
	}
}
