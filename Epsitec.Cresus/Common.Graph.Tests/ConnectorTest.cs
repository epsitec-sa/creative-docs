//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Graph
{
	[TestFixture]
	public class ConnectorTest
	{
		[Test]
		public void CheckSendData()
		{
			Assert.AreEqual (0, ConnectorTest.GraphConnectorSendData (System.IntPtr.Zero, @"S:\test.bla", null, "abc"));
			Assert.AreEqual (0, ConnectorTest.GraphConnectorSendData (System.IntPtr.Zero, @"S:\test.bla", null, "def"));
		}

		[DllImport ("GraphConnector.Win32.dll")]
		private extern static int GraphConnectorSendData(System.IntPtr handle, string path, string meta, string data);
	}
}
