//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
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
        [Ignore("Reported broken by Marc Bettex")]
        public void CheckPrintToMetafile()
        {
            PrintPort.PrintToMetafile(
                port =>
                {
                    port.Color = Color.FromName("Blue");
                    port.PaintText(20, 20, "Hello, EMF world", Font.DefaultFont, 50);
                },
                @"F:\test.emf",
                1000,
                1000
            );
        }
    }
}
