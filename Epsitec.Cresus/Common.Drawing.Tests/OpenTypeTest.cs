using NUnit.Framework;

using Epsitec.Common.Widgets;
using Epsitec.Common.Text;


namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class OpenTypeTest
	{
		[Test] public void CheckFeatures()
		{
			Common.OpenType.Tests.CheckTables.RunTests ();
		}
	}
}

