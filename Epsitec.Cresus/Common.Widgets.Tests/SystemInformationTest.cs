using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class SystemInformationTest
	{
		[Test] public void CheckSettings()
		{
			System.Console.Out.WriteLine ("DoubleClickDelay:             {0}", SystemInformation.DoubleClickDelay);
			System.Console.Out.WriteLine ("DoubleClickRadius2:           {0}", SystemInformation.DoubleClickRadius2);
			System.Console.Out.WriteLine ("CursorBlinkDelay:             {0}", SystemInformation.CursorBlinkDelay);
			System.Console.Out.WriteLine ("InitialKeyboardDelay:         {0}", SystemInformation.InitialKeyboardDelay);
			System.Console.Out.WriteLine ("KeyboardRepeatPeriod:         {0}", SystemInformation.KeyboardRepeatPeriod);
			System.Console.Out.WriteLine ("MenuShowDelay:                {0}", SystemInformation.MenuShowDelay);
			System.Console.Out.WriteLine ("MenuAnimation:                {0}", SystemInformation.MenuAnimation);
			System.Console.Out.WriteLine ("PreferRightAlignedMenus:      {0}", SystemInformation.PreferRightAlignedMenus);
			System.Console.Out.WriteLine ("SupportsLayeredWindows:       {0}", SystemInformation.SupportsLayeredWindows);
			System.Console.Out.WriteLine ("IsMenuAnimationEnabled:       {0}", SystemInformation.IsMenuAnimationEnabled);
			System.Console.Out.WriteLine ("IsComboAnimationEnabled:      {0}", SystemInformation.IsComboAnimationEnabled);
			System.Console.Out.WriteLine ("IsSmoothScrollEnabled:        {0}", SystemInformation.IsSmoothScrollEnabled);
			System.Console.Out.WriteLine ("IsMetaUnderlineEnabled:       {0}", SystemInformation.IsMetaUnderlineEnabled);
			System.Console.Out.WriteLine ("IsMenuFadingEnabled:          {0}", SystemInformation.IsMenuFadingEnabled);
			System.Console.Out.WriteLine ("IsMenuSelectionFadingEnabled: {0}", SystemInformation.IsMenuSelectionFadingEnabled);
			System.Console.Out.WriteLine ("IsMenuShadowEnabled:          {0}", SystemInformation.IsMenuShadowEnabled);
		}
	}
}
