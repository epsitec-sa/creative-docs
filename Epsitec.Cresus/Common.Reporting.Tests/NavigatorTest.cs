//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	[TestFixture]
	public class NavigatorTest
	{
		[Test]
		public void Check01CreateNavigator()
		{
			DataNavigator navigator = new DataNavigator ();
		}
	}
}
