using NUnit.Framework;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class WidgetTest
	{
		static WidgetTest()
		{
		}
		
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
			window.Root.SetAutoMinMax (true);
			
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
			System.Console.Out.WriteLine ("{0} widgets alive before GC.Collect", Widget.DebugAliveWidgetsCount);
			System.GC.Collect ();
			System.Console.Out.WriteLine ("{0} widgets alive after GC.Collect", Widget.DebugAliveWidgetsCount);
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
		
		[Test] public void CheckCommandName()
		{
			Widget w1 = new Widget ();	w1.Name = "a";
			Widget w2 = new Widget ();	w2.Name = "b";	w2.Parent = w1;
			Widget w3 = new Widget ();	w3.Name = "c";	w3.Parent = w2;
			
			Assertion.AssertEquals ("a", w1.CommandName);
			Assertion.AssertEquals ("a.b", w2.CommandName);
			Assertion.AssertEquals ("a.b.c", w3.CommandName);
		}
		
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
			Widget w1 = new Widget ();	w1.Name = "a";	w1.Parent = root;
			Widget w2 = new Widget ();					w2.Parent = root;
			Widget w3 = new Widget ();	w3.Name = "b";	w3.Parent = w2;	w3.IsCommand = true;
			Widget w4 = new Widget ();	w4.Name = "c";	w4.Parent = w2;
			Widget w5 = new Widget ();	w5.Name = "d";	w5.Parent = w4;	w5.IsCommand = true;
			Widget w6 = new Widget ();	w6.Name = "e";	w6.Parent = w1;	w6.IsCommand = true;
			
			Assertion.AssertEquals (w1, root.FindCommandWidget ("a"));
			Assertion.AssertEquals (w3, root.FindCommandWidget ("b"));
			Assertion.AssertEquals (w4, root.FindCommandWidget ("c"));
			Assertion.AssertEquals (w5, root.FindCommandWidget ("c.d"));
			Assertion.AssertEquals (w6, root.FindCommandWidget ("a.e"));
			
			Widget[] command_widgets = root.FindCommandWidgets ();
			
			Assertion.AssertEquals (3, command_widgets.Length);
		}
	}
}
