using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class PanelTest
	{
		[Test] public void CheckAbsLayout()
		{
			Window window = new Window ();
			
			Helpers.AbsPosLayoutEngine layout   = new Helpers.AbsPosLayoutEngine ();
			Design.AbsPosWidgetEdit    designer = new Design.AbsPosWidgetEdit ();
			
			Panel panel = new Panel ();
			
			panel.BackColor = Drawing.Color.FromBrightness (1.0);
			panel.Size      = new Drawing.Size (400, 300);
			
			Widget wa1   = new StaticText ("A1");	wa1.BackColor  = Drawing.Color.FromARGB (1.0, 1.0, 1.0, 0.6);	wa1.Height = 40;
			Widget wa2   = new StaticText ("A2");	wa2.BackColor  = Drawing.Color.FromARGB (1.0, 0.6, 1.0, 1.0);
			Widget wa3   = new StaticText ("A3");	wa3.BackColor  = Drawing.Color.FromARGB (1.0, 0.6, 1.0, 0.6);
			Widget wa4   = new StaticText ("A4");	wa4.BackColor  = Drawing.Color.FromARGB (1.0, 1.0, 0.8, 0.6);
			Widget wa5   = new StaticText ("A5");	wa5.BackColor  = Drawing.Color.FromARGB (1.0, 1.0, 0.6, 0.8);	wa5.Height = 60;
			
			wa1.Parent      = panel;
			wa1.Name        = "A1";
			wa1.MinSize     = new Drawing.Size (30, 30);
			wa1.Bounds      = new Drawing.Rectangle (50, 200, 150, 50);
			wa1.Anchor      = AnchorStyles.Bottom | AnchorStyles.Left;
			
			wa2.Parent      = panel;
			wa2.Name        = "A2";
			wa2.MinSize     = new Drawing.Size (30, 30);
			wa2.Bounds      = new Drawing.Rectangle (200, 200, 150, 50);
			wa2.Anchor      = AnchorStyles.Bottom | AnchorStyles.Left;
			
			wa3.Parent      = panel;
			wa3.Name        = "A3";
			wa3.MinSize     = new Drawing.Size (30, 30);
			wa3.Bounds      = new Drawing.Rectangle (50, 125, 300, 50);
			wa3.Anchor      = AnchorStyles.Bottom | AnchorStyles.Left;
			
			wa4.Parent      = panel;
			wa4.Name        = "A4";
			wa4.MinSize     = new Drawing.Size (30, 30);
			wa4.Bounds      = new Drawing.Rectangle (50, 50, 100, 50);
			wa4.Anchor      = AnchorStyles.Bottom | AnchorStyles.Left;
			
			wa5.Parent      = panel;
			wa5.Name        = "A5";
			wa5.MinSize     = new Drawing.Size (30, 30);
			wa5.Bounds      = new Drawing.Rectangle (250, 50, 100, 50);
			wa5.Anchor      = AnchorStyles.Bottom | AnchorStyles.Left;
			
			layout.Panel = panel;
			designer.Panel = panel;
			
			Scrollable surface = new Scrollable ();
			
			surface.Parent = window.Root;
			surface.Dock   = DockStyle.Fill;
			surface.Size   = new Drawing.Size (400, 300);
			surface.Panel  = panel;
			
			window.Root.IsEditionDisabled = true;
			
			window.Text = "PanelTest.CheckAbsLayout";
			window.ClientSize = new Drawing.Size (400, 300);
			window.Show ();
		}
		
		[Test] public void CheckScrollablePanel()
		{
			Window window = new Window ();
			
			window.Text = "PanelTest.CheckScrollablePanel";
			window.ClientSize = new Drawing.Size (400, 300);
			
			ScrollablePanel panel = new ScrollablePanel ();
			
			panel.Parent = window.Root;
			panel.Bounds = window.Root.Client.Bounds;
			panel.Dock   = DockStyle.Fill;
			panel.Panel.MinSize = panel.Client.Size;
			
			panel.SuspendLayout ();
			
			Button b1 = new Button ("Button 1");
			Button b2 = new Button ("Button 2");
			
			b1.Location = new Drawing.Point (10, 20);
			b2.Location = new Drawing.Point (b1.Right + 10, b1.Bottom);
			
			panel.Panel.Children.Add (b1);
			b2.Parent = panel.Panel;
			
			Assertion.Assert (b1.Parent == panel.Panel);
			Assertion.Assert (b2.Parent == panel.Panel);
			Assertion.Assert (panel.Panel.Parent == panel);
			Assertion.Assert (panel.Panel.Children.Count == 2);
			
			panel.ResumeLayout ();
			
			System.Console.Out.WriteLine ("Panel DesiredSize = {0}", panel.Panel.DesiredSize);
			System.Console.Out.WriteLine ("Panel Bounds = {0}", panel.Panel.Bounds);
			System.Console.Out.WriteLine ("Button Bounds = {0}, {1}", b1.Bounds, b2.Bounds);
			System.Console.Out.WriteLine ("Button Bounds (root relative) = {0}, {1}", b1.MapClientToRoot (b1.Client.Bounds), b2.MapClientToRoot (b2.Client.Bounds));
			
			window.Show ();
			window.SynchronousRepaint ();
			
			System.Console.Out.WriteLine ("Panel DesiredSize = {0}", panel.Panel.DesiredSize);
			System.Console.Out.WriteLine ("Panel Bounds = {0}", panel.Panel.Bounds);
			System.Console.Out.WriteLine ("Button Bounds = {0}, {1}", b1.Bounds, b2.Bounds);
			System.Console.Out.WriteLine ("Button Bounds (root relative) = {0}, {1}", b1.MapClientToRoot (b1.Client.Bounds), b2.MapClientToRoot (b2.Client.Bounds));
		}
	}
}
