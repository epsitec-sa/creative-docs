using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class WidgetTest
	{
		[Test] public void CheckParentChildRelationship()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Children.Add (widget);
			
			Assertion.Assert (root.HasChildren);
			Assertion.Assert (root.Children.Count == 1);
			Assertion.Assert (widget.HasParent);
			Assertion.AssertSame (widget.Parent, root);
			Assertion.AssertSame (root.Children[0], widget);
			
			root.Children.Remove (widget);
			
			Assertion.Assert (root.HasChildren == false);
			
			widget.Parent = root;
			
			Assertion.Assert (root.HasChildren);
			Assertion.Assert (root.Children.Count == 1);
			Assertion.Assert (widget.HasParent);
			Assertion.AssertSame (widget.Parent, root);
			Assertion.AssertSame (root.Children[0], widget);
		}
		
		[Test] public void CheckParentChanged()
		{
			Widget w0 = new Widget ();
			Widget w1 = new Widget ();
			Widget w2 = new Widget ();
			
			w2.ParentChanged += new EventHandler (HandleCheckParentChangedParentChanged);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w2.Parent = w1;
			Assertion.AssertEquals (w2, this.check_parent_changed_sender);
			Assertion.AssertEquals (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w2.Parent = null;
			Assertion.AssertEquals (w2, this.check_parent_changed_sender);
			Assertion.AssertEquals (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w1.Children.Add (w2);
			Assertion.AssertEquals (w2, this.check_parent_changed_sender);
			Assertion.AssertEquals (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w0.Children.Add (w2);
			Assertion.AssertEquals (w2, this.check_parent_changed_sender);
			Assertion.AssertEquals (2, this.check_parent_changed_count);
		}
		
		
		#region CheckParentChanged event handler
		private object		check_parent_changed_sender;
		private int			check_parent_changed_count;
		
		private void HandleCheckParentChangedParentChanged(object sender)
		{
			this.check_parent_changed_sender = sender;
			this.check_parent_changed_count++;
		}
		#endregion
		
		[Test] public void CheckAnchor()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Bounds = new Rectangle (0, 0, 120, 50);
			
			root.Children.Add (widget);
			
			widget.Anchor = AnchorStyles.Left;
			widget.Bounds = new Rectangle (20, 10, 80, 30);
			
			Assertion.Assert (widget.Anchor == AnchorStyles.Left);
			
			root.Width = 140;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			root.Width = 120;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			widget.Anchor = AnchorStyles.Right;
			widget.Bounds = new Rectangle (20, 10, 80, 30);
			
			Assertion.Assert (widget.Anchor == AnchorStyles.Right);
			
			root.Width = 140;
			Assertion.Assert ("AnchorStyles.Right, widget.Left not OK", widget.Left == 40);
			Assertion.Assert ("AnchorStyles.Right, widget.Width not OK", widget.Width == 80);
			
			root.Width = 120;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			//	TODO: ...tests additionnels...
		}
		
		
		[Test] public void CheckText()
		{
			Widget widget = new Widget ();
			string text = "Hel<m>l</m>o";
			widget.Text = text;
			Assertion.Assert (widget.Text == text);
			Assertion.Assert (widget.Mnemonic == 'L');
			widget.Text = null;
			Assertion.Assert (widget.Text == "");
		}
		
		[Test] public void CheckTextLayoutInfo()
		{
			Window window = new Window ();
			window.ClientSize = new Size (200, 120);
			window.Text       = "CheckTextLayoutInfo";
			
			StaticText text   = new StaticText ("Abcdefgh... Abcdefgh...");
			
			text.SetClientZoom (3);
			
			text.Size     = new Drawing.Size (text.PreferredSize.Width / 2, text.PreferredSize.Height * 2) * 3;
			text.Location = new Drawing.Point (10, window.ClientSize.Height - 10 - text.Height);
			text.Anchor   = AnchorStyles.TopLeft;
			text.Parent   = window.Root;
			text.PaintForeground += new PaintEventHandler(this.CheckTextLayoutInfoPaintForeground);
			
			window.Show ();
		}
		
		
		[Test] public void CheckPointMath()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			Assertion.Assert (widget.Left == 30);
			Assertion.Assert (widget.Right == 80);
			Assertion.Assert (widget.Top == 60);
			Assertion.Assert (widget.Bottom == 20);
			Assertion.Assert (widget.Client.Width == 50);
			Assertion.Assert (widget.Client.Height == 40);
			
			root.Children.Add (widget);
			
			Assertion.Assert (widget.Left == 30);
			Assertion.Assert (widget.Right == 80);
			Assertion.Assert (widget.Top == 60);
			Assertion.Assert (widget.Bottom == 20);
			
			Point pt_test   = new Point (40, 35);
			Point pt_client = widget.MapParentToClient (pt_test);
			Point pt_widget = widget.MapClientToParent (pt_client);
			
			Assertion.Assert (pt_client.X == 10);
			Assertion.Assert (pt_client.Y == 15);
			Assertion.Assert (pt_widget.X == pt_test.X);
			Assertion.Assert (pt_widget.Y == pt_test.Y);
			
			widget.SetClientAngle (90);
			
			Assertion.Assert (widget.Client.Angle == 90);
			Assertion.Assert (widget.Client.Width == 40);
			Assertion.Assert (widget.Client.Height == 50);
			
			widget.SetClientAngle (180);
			widget.SetClientZoom (0.5f);
			
			Assertion.Assert (widget.Client.Angle == 180);
			Assertion.Assert (widget.Client.Width == 100);
			Assertion.Assert (widget.Client.Height == 80);
			
			widget.SetClientAngle (180);
			widget.SetClientZoom (0.5f);
			
			pt_test   = new Point (widget.Left, widget.Bottom);
			pt_client = widget.MapParentToClient (pt_test);
			pt_widget = widget.MapClientToParent (pt_client);
			
			Assertion.Assert (Transform.Equal (pt_widget, pt_test));
			Assertion.Assert (Transform.Equal (pt_client.X, widget.Client.Width));
			Assertion.Assert (Transform.Equal (pt_client.Y, widget.Client.Height));
			
			
			double zoom = 2.0;
			double ox = 1.0;
			double oy = 2.0;
			
			for (int angle = 0; angle < 360; angle += 90)
			{
				widget.SetClientAngle (angle);
				widget.SetClientZoom (zoom);
			
				pt_test   = new Point (widget.Left + ox, widget.Bottom + oy);
				pt_client = widget.MapParentToClient (pt_test);
				pt_widget = widget.MapClientToParent (pt_client);
			
				Assertion.Assert (Transform.Equal (pt_widget, pt_test));
				
				switch (angle)
				{
					case 0:
						Assertion.Assert ("0° failed", Transform.Equal (pt_client.X, ox / zoom));
						Assertion.Assert ("0° failed", Transform.Equal (pt_client.Y, oy / zoom));
						break;
					
					case 90:
						Assertion.Assert ("90° failed", Transform.Equal (pt_client.X, oy / zoom));
						Assertion.Assert ("90° failed", Transform.Equal (pt_client.Y, widget.Client.Height - ox / zoom));
						break;
					
					case 180:
						Assertion.Assert ("180° failed", Transform.Equal (pt_client.X, (widget.Client.Width - ox / zoom)));
						Assertion.Assert ("180° failed", Transform.Equal (pt_client.Y, (widget.Client.Height - oy / zoom)));
						break;
					
					case 270:
						Assertion.Assert ("270° failed", Transform.Equal (pt_client.X, (widget.Client.Width - oy / zoom)));
						Assertion.Assert ("270° failed", Transform.Equal (pt_client.Y, ox / zoom));
						break;
				}
			}
		}
		
		[Test] public void CheckTransformToClient()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
			double ox = 1.0;
			double oy = 2.0;
			
			Point pt1 = new Point (widget.Left + ox, widget.Bottom + oy);
			Point pt2;
			Point pt3;
			
			widget.SetClientAngle (0);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientAngle (180);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientAngle (270);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void CheckTransformRectToClient()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			root.Children.Add (widget);
			
			double ox = 1.0;
			double oy = 2.0;
			double dx = 10.0;
			double dy = 6.0;
			
			Rectangle rect1 = new Rectangle (widget.Left + ox, widget.Bottom + oy, dx, dy);
			Rectangle rect2;
			Point pt1 = new Point (rect1.Left,  rect1.Bottom);
			Point pt2 = new Point (rect1.Right, rect1.Top);
			Point pt3;
			Point pt4;
			
			widget.SetClientAngle (0);
			widget.SetClientZoom (2);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (7);
			widget.SetClientAngle (180);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (1.5f);
			widget.SetClientAngle (270);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
		}
		
		[Test] public void CheckTransformRectToParent()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			root.Children.Add (widget);
			
			double ox = 1.0;
			double oy = 2.0;
			double dx = 10.0;
			double dy = 6.0;
			
			Rectangle rect1 = new Rectangle (widget.Left + ox, widget.Bottom + oy, dx, dy);
			Rectangle rect2;
			Point pt1 = new Point (rect1.Left,  rect1.Bottom);
			Point pt2 = new Point (rect1.Right, rect1.Top);
			Point pt3;
			Point pt4;
			
			widget.SetClientAngle (0);
			widget.SetClientZoom (2);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (7);
			widget.SetClientAngle (180);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (1.5f);
			widget.SetClientAngle (270);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assertion.Assert (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assertion.Assert (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assertion.Assert (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
		}
		
		[Test] public void CheckTransformToParent()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
			double ox = 1.0;
			double oy = 2.0;
			
			Point pt1 = new Point (ox, oy);
			Point pt2;
			Point pt3;
			
			widget.SetClientAngle (0);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (180);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (270);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void CheckTransformParentClientIdentity()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (0, 0);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Drawing.Transform identity  = new Epsitec.Common.Drawing.Transform ();
			Epsitec.Common.Drawing.Transform transform = new Epsitec.Common.Drawing.Transform ();
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			transform = widget.GetTransformToClient ();
			transform.MultiplyBy (widget.GetTransformToParent ());
			
			Assertion.Assert (identity.Equals (transform));
			
			transform = widget.GetTransformToParent ();
			transform.MultiplyBy (widget.GetTransformToClient ());
			
			Assertion.Assert (identity.Equals (transform));
		}
		
		[Test] public void CheckTransformHierarchy()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new Point (100, 150);
			root.Size     = new Size (300, 200);
			
			widget.Location = new Point (30, 20);
			widget.Size     = new Size (50, 40);
			widget.Parent   = root;
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			Epsitec.Common.Drawing.Transform t1 = widget.GetRootToClientTransform ();
			Epsitec.Common.Drawing.Transform t2 = widget.GetClientToRootTransform ();
			
			System.Console.Out.WriteLine ("root -> client : " + t1.ToString ());
			System.Console.Out.WriteLine ("client -> root : " + t2.ToString ());
			
			Assertion.Assert (Epsitec.Common.Drawing.Transform.Multiply (t1, t2).Equals (new Epsitec.Common.Drawing.Transform ()));
		}
		
		[Test] public void CheckDocking()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckDocking";
			window.Root.PreferHorizontalDockLayout = true;
			window.Root.AutoMinMax = true;
			
			Button button;
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "A";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "B";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "C";
			button.Dock = DockStyle.Right;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "D";
			button.Dock = DockStyle.Top;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "E";
			button.Dock = DockStyle.Bottom;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "F";
			button.Dock = DockStyle.Left;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "G";
			button.Dock = DockStyle.Right;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "H";
			button.Dock = DockStyle.Fill;
			window.Root.Children.Add(button);
			
			button = new Button();
			button.Size = new Size(40, 24);
			button.Text = "I";
			button.Dock = DockStyle.Fill;
			window.Root.Children.Add(button);
			
			window.Show ();
		}
		
		[Test] public void CheckAliveWidgets()
		{
			System.Console.Out.WriteLine ("{0} widgets and {1} windows alive before GC.Collect", Widget.DebugAliveWidgetsCount, Window.DebugAliveWindowsCount);
			System.GC.Collect ();
			System.Console.Out.WriteLine ("{0} widgets and {1} windows alive after GC.Collect", Widget.DebugAliveWidgetsCount, Window.DebugAliveWindowsCount);
			foreach (Window window in Window.DebugAliveWindows)
			{
				System.Console.Out.WriteLine ("{0}: Name='{1}', Text='{2}'", window.GetType ().Name, window.Name, window.Text);
			}
			foreach (Widget widget in Widget.DebugAliveWidgets)
			{
				System.Console.Out.WriteLine ("{0}: Name='{1}', Text='{2}', Parent={3}", widget.GetType ().Name, widget.Name, widget.Text, (widget.Parent == null) ? "<null>" : (widget.Parent.GetType ().Name));
			}
		}
		
		[Test] public void CheckButtonNew()
		{
			Button button = new Button ();
		}
		
		[Test] public void CheckButtonNewDispose()
		{
			Button button = new Button ();
			button.Dispose ();
		}
		
		[Test] public void CheckButtonNewGC()
		{
			Button button = new Button ();
			button = null;
			System.GC.Collect ();
		}
		
		[Test] public void CheckFullPathName()
		{
			Widget w1 = new Widget (); w1.Name = "XA";
			Widget w2 = new Widget (); w2.Name = "XB"; w2.Parent = w1;
			Widget w3 = new Widget (); w3.Name = "XC"; w3.Parent = w2;
			
			Assertion.AssertEquals ("XA", w1.FullPathName);
			Assertion.AssertEquals ("XA.XB", w2.FullPathName);
			Assertion.AssertEquals ("XA.XB.XC", w3.FullPathName);
			
			Assertion.AssertEquals (w2, w1.FindChildByPath ("XA.XB"));
			Assertion.AssertEquals (w3, w1.FindChildByPath ("XA.XB.XC"));
			
			Widget[] find = Widget.FindAllFullPathWidgets (Support.RegexFactory.FromSimpleJoker ("*XB*"));
			
			Assertion.AssertEquals (2, find.Length);
			Assertion.AssertEquals (w2, find[0]);
			Assertion.AssertEquals (w3, find[1]);
		}
		
		[Test] public void CheckCommandName()
		{
			Widget w1 = new Widget (); w1.Name = "A"; w1.CommandName = "a";
			Widget w2 = new Widget (); w2.Name = "B"; w2.CommandName = "b"; w2.Parent = w1;
			Widget w3 = new Widget (); w3.Name = "C"; w3.CommandName = "c"; w3.Parent = w2;
			
			Assertion.AssertEquals ("a", w1.CommandName);
			Assertion.AssertEquals ("b", w2.CommandName);
			Assertion.AssertEquals ("c", w3.CommandName);
			
			Assertion.Assert (w1.IsCommand);
			Assertion.Assert (w2.IsCommand);
			Assertion.Assert (w3.IsCommand);
			
			Assertion.AssertEquals (w2, w1.FindChildByPath ("A.B"));
			Assertion.AssertEquals (w3, w1.FindChildByPath ("A.B.C"));
			
			Widget[] find = w1.FindCommandWidgets ("b");
			
			Assertion.AssertEquals (1, find.Length);
			Assertion.AssertEquals (w2, find[0]);
		}
		
		[Test] public void CheckFindAllCommandNames()
		{
			Widget[] find = Widget.FindAllCommandWidgets (Support.RegexFactory.FromSimpleJoker ("*"));
			
			for (int i = 0; i < find.Length; i++)
			{
				System.Console.Out.WriteLine ("{0} : '{1}' in {2}", i, find[i].CommandName, find[i].ToString ());
			}
		}
		
		[Test] public void CheckSaveCommandEnable()
		{
			Widget[] find = Widget.FindAllCommandWidgets (Support.RegexFactory.FromSimpleJoker ("*.save"));
			
			for (int i = 0; i < find.Length; i++)
			{
				find[i].SetEnabled (true);
				find[i].Invalidate ();
			}
		}
		
		[Test] public void CheckSaveCommandDisable()
		{
			Widget[] find = Widget.FindAllCommandWidgets (Support.RegexFactory.FromSimpleJoker ("*.save"));
			
			for (int i = 0; i < find.Length; i++)
			{
				find[i].SetEnabled (false);
				find[i].Invalidate ();
			}
		}
		
		[Test] public void CheckCommandState()
		{
			WidgetTest.open_state.Enabled = ! WidgetTest.open_state.Enabled;
		}
		
		static CommandState open_state = new CommandState ("*.open");
		
		[Test] public void CheckFindChildBasedOnName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.Name = "a";	w1.Parent = root;
			Widget w2 = new Widget ();					w2.Parent = root;
			Widget w3 = new Widget ();	w3.Name = "b";	w3.Parent = w2;
			
			Assertion.AssertEquals (w1, root.FindChild ("a"));
			Assertion.AssertEquals (w3, root.FindChild ("b"));
		}
		
		[Test] public void CheckFindChildBasedOnCommandName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.CommandName = "a";	w1.Parent = root;
			Widget w2 = new Widget ();							w2.Parent = root;
			Widget w3 = new Widget ();	w3.CommandName = "b";	w3.Parent = w2;
			Widget w4 = new Widget ();	w4.CommandName = "c";	w4.Parent = w2;
			Widget w5 = new Widget ();	w5.CommandName = "d";	w5.Parent = w4;
			Widget w6 = new Widget ();	w6.Name = "e";			w6.Parent = w1;
			
			Assertion.AssertEquals (w1, root.FindCommandWidgets ("a") [0]);
			Assertion.AssertEquals (w3, root.FindCommandWidgets ("b") [0]);
			Assertion.AssertEquals (w4, root.FindCommandWidgets ("c") [0]);
			Assertion.AssertEquals (w5, root.FindCommandWidgets ("d") [0]);
			
			Assertion.Assert (root.FindCommandWidgets ("e").Length == 0);
			
			Widget[] command_widgets = root.FindCommandWidgets ();
			
			Assertion.AssertEquals (4, command_widgets.Length);
		}
		
		
		
		private void CheckTextLayoutInfoPaintForeground(object sender, PaintEventArgs e)
		{
			Widget widget = sender as Widget;
			
			Drawing.Point pos;
			
			double ascender;
			double descender;
			double width;
			
			widget.TextLayout.GetLineGeometry (0, out pos, out ascender, out descender, out width);
			
			Drawing.Path path = new Drawing.Path ();
			
			path.MoveTo (pos.X, pos.Y);
			path.LineTo (pos.X + width, pos.Y);
			path.MoveTo (pos.X, pos.Y + ascender);
			path.LineTo (pos.X + width, pos.Y + ascender);
			path.MoveTo (pos.X, pos.Y + descender);
			path.LineTo (pos.X + width, pos.Y + descender);
			
			e.Graphics.Rasterizer.AddOutline (path, 0.2);
			e.Graphics.RenderSolid (Drawing.Color.FromRGB (1, 0, 0));
		}
	}
}
