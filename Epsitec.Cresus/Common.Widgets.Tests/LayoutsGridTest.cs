using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class LayoutsGridTest
	{
		[Test] public void CheckSetup()
		{
			Layouts.Grid layout = new Layouts.Grid ();
			
			Widget wa   = new Widget ();
			Widget wa1  = new Widget ();
			Widget wa11 = new Widget ();
			Widget wa2  = new Widget ();
			Widget wa3  = new Widget ();
			
			layout.Columns.Add (new Layouts.Grid.Column (50, 1));
			layout.Columns.Add (new Layouts.Grid.Column (50, 0));
			layout.Columns.Add (new Layouts.Grid.Column (50, 1));
			
			double[] min_dx;
			double[] live_dx;
			
			//	0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15
			//	[A1 [A11  40]              100][A2         50]
			//	[A3                                    140]
			//	x           x                 x              x
			// (0)         (1)               (2)            (3)
			
			wa1.Parent      = wa;
			wa1.Name        = "A1";
			wa1.MinSize     = new Drawing.Size (100, 20);
			wa1.Dock        = DockStyle.Layout;
			wa1.LayoutFlags = LayoutFlags.IncludeChildren;
			wa1.SetLayoutArgs (0, 2);
			
			wa11.Parent     = wa1;
			wa11.Name       = "A11";
			wa11.MinSize    = new Drawing.Size (40, 20);
			wa11.Dock       = DockStyle.Layout;
			wa11.SetLayoutArgs (0, 1);
			
			wa2.Parent      = wa;
			wa2.Name        = "A2";
			wa2.MinSize     = new Drawing.Size (50, 20);
			wa2.Dock        = DockStyle.Layout;
			wa2.SetLayoutArgs (2, 3);
			
			wa3.Parent      = wa;
			wa3.Name        = "A3";
			wa3.MinSize     = new Drawing.Size (140, 20);
			wa3.LayoutFlags = LayoutFlags.StartNewLine;
			wa3.Dock        = DockStyle.Layout;
			wa3.SetLayoutArgs (0, 3);
			
			layout.Root = wa;
			layout.DesiredWidth = 100;
			
			min_dx  = layout.MinColumnWidths;
			live_dx = layout.CurrentColumnWidths;
			
			Assertion.AssertEquals (3, min_dx.Length);
			Assertion.AssertEquals (3, live_dx.Length);
			Assertion.AssertEquals (40, min_dx[0]);
			Assertion.AssertEquals (60, min_dx[1]);
			Assertion.AssertEquals (50, min_dx[2]);
			Assertion.AssertEquals (40, live_dx[0]);
			Assertion.AssertEquals (60, live_dx[1]);
			Assertion.AssertEquals (50, live_dx[2]);
			Assertion.AssertEquals (100, layout.DesiredWidth);
			Assertion.AssertEquals (150, layout.CurrentWidth);
			
			layout.DesiredWidth = 200;
			live_dx = layout.CurrentColumnWidths;
			
			Assertion.AssertEquals (3, live_dx.Length);
			Assertion.AssertEquals (70, live_dx[0]);
			Assertion.AssertEquals (60, live_dx[1]);
			Assertion.AssertEquals (70, live_dx[2]);
			Assertion.AssertEquals (200, layout.DesiredWidth);
			Assertion.AssertEquals (200, layout.CurrentWidth);
			
			
			wa3.MinSize = new Drawing.Size (160, 20);
			wa3.SetLayoutArgs (0, 3);
			layout.Invalidate ();
			min_dx = layout.MinColumnWidths;
			
			Assertion.AssertEquals (3, min_dx.Length);
			Assertion.AssertEquals (40, min_dx[0]);
			Assertion.AssertEquals (60, min_dx[1]);
			Assertion.AssertEquals (60, min_dx[2]);
			
			wa3.MinSize = new Drawing.Size (80, 20);
			wa3.SetLayoutArgs (1, 2);
			layout.Invalidate ();
			min_dx = layout.MinColumnWidths;
			
			Assertion.AssertEquals (3, min_dx.Length);
			Assertion.AssertEquals (40, min_dx[0]);
			Assertion.AssertEquals (80, min_dx[1]);
			Assertion.AssertEquals (50, min_dx[2]);
		}
	}
}
