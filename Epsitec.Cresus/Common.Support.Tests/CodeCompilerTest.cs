//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class CodeCompilerTest
	{
		[Test]
		public void CheckBuildDriverIsValidInstallation()
		{
			BuildDriver driver = new BuildDriver ();

			Assert.IsTrue (driver.IsValidInstallation);
		}
	}
}
