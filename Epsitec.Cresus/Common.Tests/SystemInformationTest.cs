using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class SystemInformationTest
	{
		[Test] public void CheckSettings()
		{
			System.Console.Out.WriteLine ("DoubleClickDelay:        {0}", SystemInformation.DoubleClickDelay);
			System.Console.Out.WriteLine ("DoubleClickRadius2:      {0}", SystemInformation.DoubleClickRadius2);
			System.Console.Out.WriteLine ("CursorBlinkDelay:        {0}", SystemInformation.CursorBlinkDelay);
			System.Console.Out.WriteLine ("InitialKeyboardDelay:    {0}", SystemInformation.InitialKeyboardDelay);
			System.Console.Out.WriteLine ("KeyboardRepeatPeriod:    {0}", SystemInformation.KeyboardRepeatPeriod);
			System.Console.Out.WriteLine ("MenuShowDelay:           {0}", SystemInformation.MenuShowDelay);
			System.Console.Out.WriteLine ("PreferRightAlignedMenus: {0}", SystemInformation.PreferRightAlignedMenus);
			System.Console.Out.WriteLine ("SupportsLayeredWindows:  {0}", SystemInformation.SupportsLayeredWindows);
		}
	}
}
