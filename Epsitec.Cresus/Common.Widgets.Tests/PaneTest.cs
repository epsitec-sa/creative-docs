using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class PaneTest
	{
		[Test] public void CheckPane()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (400, 300);
			window.Text = "CheckPane";
			
			Pane pane = new Pane ();
			
			pane.PaneStyle = PaneStyle.LeftRight;
			pane.PaneSizeChanged += new EventHandler(this.pane_SizeChanged);
			pane.Bounds = window.Root.Client.Bounds;
			pane.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			pane.LayoutChanged += new EventHandler(pane_LayoutChanged);
			
			window.Root.Children.Add (pane);
			
			StaticText title;
			
			title = new StaticText ();
			title.SetClientZoom (3);
			title.Text = "<b>Test Pane #0</b>";
			title.Bounds = pane.RetPane (0).Client.Bounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			pane.RetPane (0).Children.Add (title);
			
			title = new StaticText ();
			title.SetClientZoom (3);
			title.Text = "<b>Test Pane #1</b>";
			title.Bounds = pane.RetPane (1).Client.Bounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			pane.RetPane (1).Children.Add (title);

			window.Show();
		}
		
		private void pane_SizeChanged(object sender)
		{
			Pane pane = sender as Pane;
			
			double w0 = pane.RetSize (0);
			double w1 = pane.RetSize (1);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Pane resized: {0} / {1}", w0, w1));
		}

		private void pane_LayoutChanged(object sender)
		{
			Pane pane = sender as Pane;
			
			System.Diagnostics.Debug.WriteLine ("Layout Changed: " + pane.Client.Size.ToString ());
		}
	}
}
