using System;
using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class WidgetTest
	{
		static WidgetTest()
		{
			try { System.Diagnostics.Debug.WriteLine (""); } catch { }
		}
		
		[Test] [Ignore ("Not implemented yet")] public void TestParentChildRelationship()
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
		
		[Test] [Ignore ("Not implemented yet")] public void TestAnchor()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Bounds = new System.Drawing.RectangleF (0, 0, 120, 50);
			
			root.Children.Add (widget);
			
			widget.Anchor = Widget.AnchorStyles.Left;
			widget.Bounds = new System.Drawing.RectangleF (20, 10, 80, 30);
			
			Assertion.Assert (widget.Anchor == Widget.AnchorStyles.Left);
			
			root.Width = 140;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			root.Width = 120;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			widget.Anchor = Widget.AnchorStyles.Right;
			widget.Bounds = new System.Drawing.RectangleF (20, 10, 80, 30);
			
			Assertion.Assert (widget.Anchor == Widget.AnchorStyles.Right);
			
			root.Width = 140;
			Assertion.Assert ("AnchorStyles.Right, widget.Left not OK", widget.Left == 40);
			Assertion.Assert ("AnchorStyles.Right, widget.Width not OK", widget.Width == 80);
			
			root.Width = 120;
			Assertion.Assert (widget.Left == 20);
			Assertion.Assert (widget.Width == 80);
			
			//	TODO: ...tests additionnels...
		}
		
		
		[Test] public void TestText()
		{
			Widget widget = new Widget ();
			string text = "Hel&Lo";
			widget.Text = text;
			Assertion.Assert (widget.Text == text);
			Assertion.Assert (widget.Mnemonic == 'l');
			widget.Text = null;
			Assertion.Assert (widget.Text == "");
		}
		
		[Test] public void TestPointMath()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new System.Drawing.PointF (0, 0);
			root.Size     = new System.Drawing.SizeF (300, 200);
			
			widget.Location = new System.Drawing.PointF (30, 20);
			widget.Size     = new System.Drawing.SizeF (50, 40);
			
			Assertion.Assert (widget.Left == 30);
			Assertion.Assert (widget.Right == 80);
			Assertion.Assert (widget.Top == 20);
			Assertion.Assert (widget.Bottom == 60);
			Assertion.Assert (widget.Client.Width == 50);
			Assertion.Assert (widget.Client.Height == 40);
			
			root.Children.Add (widget);
			
			Assertion.Assert (widget.Left == 30);
			Assertion.Assert (widget.Right == 80);
			Assertion.Assert (widget.Top == 20);
			Assertion.Assert (widget.Bottom == 60);
			
			System.Drawing.PointF pt_test   = new System.Drawing.PointF (40, 35);
			System.Drawing.PointF pt_client = widget.MapParentToClient (pt_test);
			System.Drawing.PointF pt_widget = widget.MapClientToParent (pt_client);
			
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
			
			pt_test   = widget.Location;
			pt_client = widget.MapParentToClient (pt_test);
			pt_widget = widget.MapClientToParent (pt_client);
			
			Assertion.Assert (pt_widget.X == pt_test.X);
			Assertion.Assert (pt_widget.Y == pt_test.Y);
			Assertion.Assert (pt_client.X == widget.Client.Width);
			Assertion.Assert (pt_client.Y == widget.Client.Height);
			
			
			float zoom = 2.0f;
			float ox = 1.0f;
			float oy = 2.0f;
			
			for (int angle = 0; angle < 360; angle += 90)
			{
				widget.SetClientAngle (angle);
				widget.SetClientZoom (zoom);
			
				pt_test   = new System.Drawing.PointF (widget.Left + ox, widget.Top + oy);
				pt_client = widget.MapParentToClient (pt_test);
				pt_widget = widget.MapClientToParent (pt_client);
			
				Assertion.Assert (pt_widget.X == pt_test.X);
				Assertion.Assert (pt_widget.Y == pt_test.Y);
				
				switch (angle)
				{
					case 0:
						Assertion.Assert ("0° failed", pt_client.X == ox / zoom);
						Assertion.Assert ("0° failed", pt_client.Y == oy / zoom);
						break;
					
					case 90:
						Assertion.Assert ("90° failed", pt_client.X == oy / zoom);
						Assertion.Assert ("90° failed", pt_client.Y == (widget.Client.Height - ox / zoom));
						break;
					
					case 180:
						Assertion.Assert ("180° failed", pt_client.X == (widget.Client.Width - ox / zoom));
						Assertion.Assert ("180° failed", pt_client.Y == (widget.Client.Height - oy / zoom));
						break;
					
					case 270:
						Assertion.Assert ("270° failed", pt_client.X == (widget.Client.Width - oy / zoom));
						Assertion.Assert ("270° failed", pt_client.Y == ox / zoom);
						break;
				}
			}
		}
		
		[Test] public void TestTransformToClient()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new System.Drawing.PointF (0, 0);
			root.Size     = new System.Drawing.SizeF (300, 200);
			
			widget.Location = new System.Drawing.PointF (30, 20);
			widget.Size     = new System.Drawing.SizeF (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Grafix.Transform transform = new Epsitec.Common.Grafix.Transform ();
			
			float ox = 1.0f;
			float oy = 2.0f;
			
			System.Drawing.PointF pt1 = new System.Drawing.PointF (widget.Left + ox, widget.Top + oy);
			System.Drawing.PointF pt2;
			System.Drawing.PointF pt3;
			
			transform.Reset ();
			widget.SetClientAngle (0);
			widget.MergeTransformToClient (transform);
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			widget.MergeTransformToClient (transform);
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (180);
			widget.MergeTransformToClient (transform);
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (270);
			widget.MergeTransformToClient (transform);
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void TestTransformToParent()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new System.Drawing.PointF (0, 0);
			root.Size     = new System.Drawing.SizeF (300, 200);
			
			widget.Location = new System.Drawing.PointF (30, 20);
			widget.Size     = new System.Drawing.SizeF (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Grafix.Transform transform = new Epsitec.Common.Grafix.Transform ();
			
			float ox = 1.0f;
			float oy = 2.0f;
			
			System.Drawing.PointF pt1 = new System.Drawing.PointF (ox, oy);
			System.Drawing.PointF pt2;
			System.Drawing.PointF pt3;
			
			transform.Reset ();
			widget.SetClientAngle (0);
			widget.MergeTransformToParent (transform);
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			widget.MergeTransformToParent (transform);
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (180);
			widget.MergeTransformToParent (transform);
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (270);
			widget.MergeTransformToParent (transform);
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Equal (pt2, pt3));
		}
		
		[Test] public void TestTransformParentClientIdentity()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new System.Drawing.PointF (0, 0);
			root.Size     = new System.Drawing.SizeF (300, 200);
			
			widget.Location = new System.Drawing.PointF (30, 20);
			widget.Size     = new System.Drawing.SizeF (50, 40);
			
			root.Children.Add (widget);
			
			Epsitec.Common.Grafix.Transform identity  = new Epsitec.Common.Grafix.Transform ();
			Epsitec.Common.Grafix.Transform transform = new Epsitec.Common.Grafix.Transform ();
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			transform.Reset ();
			widget.MergeTransformToClient (transform);
			widget.MergeTransformToParent (transform);
			
			Assertion.Assert (identity.Equals (transform));
			
			transform.Reset ();
			widget.MergeTransformToParent (transform);
			widget.MergeTransformToClient (transform);
			
			Assertion.Assert (identity.Equals (transform));
		}
		
		[Test] public void TestTransformHierarchy()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Location = new System.Drawing.PointF (100, 150);
			root.Size     = new System.Drawing.SizeF (300, 200);
			
			widget.Location = new System.Drawing.PointF (30, 20);
			widget.Size     = new System.Drawing.SizeF (50, 40);
			widget.Parent   = root;
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			Epsitec.Common.Grafix.Transform t1 = widget.GetRootToClientTransform ();
			Epsitec.Common.Grafix.Transform t2 = widget.GetClientToRootTransform ();
			
			System.Console.Out.WriteLine ("root -> client : " + t1.ToString ());
			System.Console.Out.WriteLine ("client -> root : " + t2.ToString ());
			
			Assertion.Assert (Epsitec.Common.Grafix.Transform.Multiply (t1, t2).Equals (new Epsitec.Common.Grafix.Transform ()));
		}
	}
}
