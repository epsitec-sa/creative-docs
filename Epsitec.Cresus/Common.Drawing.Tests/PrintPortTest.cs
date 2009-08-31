//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Printing
{
	[TestFixture]
	public class PrintPortTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckPrintToMetafile()
		{
			PrintPort.PrintToMetafile (
				port =>
				{
					port.Color = Color.FromName ("Blue");
					port.PaintText (20, 20, "Hello, EMF world", Font.DefaultFont, 50);
				},
				@"F:\test.emf", 1000, 1000);
		}
	}
}
