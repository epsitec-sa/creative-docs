using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class ScreenInfoTest
	{
		[Test] public void CheckAllScreens()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			System.Console.Out.WriteLine ("Found {0} screens. GlobalArea={1}", screens.Length, ScreenInfo.GlobalArea);
			System.Console.Out.WriteLine ();
			
			foreach (ScreenInfo info in screens)
			{
				System.Console.Out.WriteLine ("{0} :", info.DeviceName);
				System.Console.Out.WriteLine (" - Bounds={0}", info.Bounds);
				System.Console.Out.WriteLine (" - WorkingArea={0}", info.WorkingArea);
				System.Console.Out.WriteLine (" - IsPrimary={0}", info.IsPrimary);
				System.Console.Out.WriteLine ();
				
				Assertion.Assert (info.Bounds.Contains (info.WorkingArea));
				Assertion.Assert (ScreenInfo.GlobalArea.Contains (info.Bounds));
			}
		}
		
		[Test] public void CheckFindPoint()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			foreach (ScreenInfo info in screens)
			{
				ScreenInfo found = ScreenInfo.Find (new Point (info.Bounds.Left + info.Bounds.Width / 2, info.Bounds.Bottom + info.Bounds.Height / 2));
				Assertion.AssertEquals (info.Bounds, found.Bounds);
			}
		}
		
		[Test] public void CheckFindRectangle()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			foreach (ScreenInfo info in screens)
			{
				Rectangle rect1 = new Rectangle (info.Bounds.X - 100, info.Bounds.Bottom + info.Bounds.Height / 2 - 100, 120, 200);
				Rectangle rect2 = new Rectangle (info.Bounds.X -  20, info.Bounds.Bottom + info.Bounds.Height / 2 - 100, 120, 200);
				
				ScreenInfo found1 = ScreenInfo.Find (rect1);
				ScreenInfo found2 = ScreenInfo.Find (rect2);
				
				System.Console.Out.WriteLine ("First rect found on {0}, Bounds={1}", found1.DeviceName, found1.Bounds.ToString ());
				System.Console.Out.WriteLine ("Second rect found on {0}, Bounds={1}", found2.DeviceName, found2.Bounds.ToString ());
				System.Console.Out.WriteLine ();
			}
		}
	}
}
