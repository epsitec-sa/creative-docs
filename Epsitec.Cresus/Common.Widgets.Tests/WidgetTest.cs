using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class WidgetTest
	{
		[Test] public void CheckTextFrame()
		{
			Window window = new Window ();
			
			window.ClientSize = new Size (400, 500);
			window.Text       = "CheckTextFrame";
			
			TextFrame      frame     = new TextFrame ();
			TextNavigator2 navigator = frame.TextNavigator;
			
			Text.TextStory   story     = frame.TextStory;
			Text.TextStyle[] no_styles = new Text.TextStyle[0];
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			Text.Properties.FontProperty fp = new Text.Properties.FontProperty ("Palatino Linotype", "Italic");
			
			fp.Features = new string[] { "liga", "dlig", "kern" };
			
			properties.Add (fp);
			properties.Add (new Text.Properties.FontSizeProperty (14.0, Text.Properties.SizeUnits.Points));
			properties.Add (new Text.Properties.MarginsProperty (60, 10, 10, 10, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
			properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties.Add (new Text.Properties.LanguageProperty ("fr-ch", 1.0));
			properties.Add (new Text.Properties.LeadingProperty (16.0, Text.Properties.SizeUnits.Points, 15.0, Text.Properties.SizeUnits.Points, 5.0, Text.Properties.SizeUnits.Points, Text.Properties.AlignMode.None));
			
			Text.TextStyle style = story.TextContext.StyleList.NewTextStyle ("Default", Text.TextStyleClass.Paragraph, properties);
			story.TextContext.DefaultStyle = style;
			
			string words = "Bonjour, ceci est un texte d'exemple permettant de vérifier le bon fonctionnement des divers algorithmes de découpe et d'affichage. Le nombre de mots moyen s'élève à environ 40 mots par paragraphe, ce qui correspond à des paragraphes de taille réduite. Quelle idée, un fjord finlandais ! Avocat.\nAWAY.\n______\n";
			
			navigator.Insert (words);
			
			frame.Dock        = DockStyle.Fill;
			frame.DockMargins = new Margins (4, 4, 4, 4);
			frame.Parent      = window.Root;
			
			frame.Focus ();
			
			window.Show ();
		}
		
		[Test] public void CheckParentChildRelationship()
		{
			Widget root = new Widget ();
			Widget widget = new Widget ();
			
			root.Children.Add (widget);
			
			Assert.IsTrue (root.HasChildren);
			Assert.IsTrue (root.Children.Count == 1);
			Assert.IsTrue (widget.HasParent);
			Assert.AreSame (widget.Parent, root);
			Assert.AreSame (root.Children[0], widget);
			
			root.Children.Remove (widget);
			
			Assert.IsTrue (root.HasChildren == false);
			
			widget.Parent = root;
			
			Assert.IsTrue (root.HasChildren);
			Assert.IsTrue (root.Children.Count == 1);
			Assert.IsTrue (widget.HasParent);
			Assert.AreSame (widget.Parent, root);
			Assert.AreSame (root.Children[0], widget);
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
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w2.Parent = null;
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w1.Children.Add (w2);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (1, this.check_parent_changed_count);
			
			this.check_parent_changed_sender = null;
			this.check_parent_changed_count  = 0;
			w0.Children.Add (w2);
			Assert.AreEqual (w2, this.check_parent_changed_sender);
			Assert.AreEqual (2, this.check_parent_changed_count);
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
			
			widget.Bounds = new Rectangle (20, 10, 80, 30);
			widget.Anchor = AnchorStyles.Left;
			widget.AnchorMargins = new Drawing.Margins (20, 0, 0, 0);
			
			Assert.IsTrue (widget.Anchor == AnchorStyles.Left);
			
			root.Width = 140;
			Assert.IsTrue (widget.Left == 20);
			Assert.IsTrue (widget.Width == 80);
			
			root.Width = 120;
			Assert.IsTrue (widget.Left == 20);
			Assert.IsTrue (widget.Width == 80);
			
			widget.Bounds = new Rectangle (20, 10, 80, 30);
			widget.Anchor = AnchorStyles.Right;
			widget.AnchorMargins = new Drawing.Margins (0, 20, 0, 0);
			
			Assert.IsTrue (widget.Anchor == AnchorStyles.Right);
			Assert.IsTrue (widget.Right == 100, "AnchorStyles.Right, widget.Right not OK");
			Assert.IsTrue (widget.Left  == 20, "AnchorStyles.Right, widget.Left not OK");
			
			root.Width = 140;
			Assert.IsTrue (widget.Left  == 40, "AnchorStyles.Right, widget.Left not OK");
			Assert.IsTrue (widget.Width == 80, "AnchorStyles.Right, widget.Width not OK");
			
			root.Width = 120;
			Assert.IsTrue (widget.Left == 20);
			Assert.IsTrue (widget.Width == 80);
			
			//	TODO: ...tests additionnels...
		}
		
		
		[Test] public void CheckText()
		{
			Widget widget = new Widget ();
			string text = "Hel<m>l</m>o";
			widget.Text = text;
			Assert.IsTrue (widget.Text == text);
			Assert.IsTrue (widget.Mnemonic == 'L');
			widget.Text = null;
			Assert.IsTrue (widget.Text == "");
		}
		
		[Test] public void CheckTextLayoutInfo()
		{
			Window window = new Window ();
			window.ClientSize = new Size (200, 120);
			window.Text       = "CheckTextLayoutInfo";
			
			StaticText text   = new StaticText ("Abcdefgh... Abcdefgh...");
			
			text.SetClientZoom (3);
			
			text.Size     = new Drawing.Size (text.PreferredSize.Width / 2, text.PreferredSize.Height * 2) * 3;
			text.Anchor   = AnchorStyles.TopLeft;
			text.AnchorMargins = new Drawing.Margins (10, 0, 10, 0);
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
			
			Assert.IsTrue (widget.Left == 30);
			Assert.IsTrue (widget.Right == 80);
			Assert.IsTrue (widget.Top == 60);
			Assert.IsTrue (widget.Bottom == 20);
			Assert.IsTrue (widget.Client.Width == 50);
			Assert.IsTrue (widget.Client.Height == 40);
			
			root.Children.Add (widget);
			
			Assert.IsTrue (widget.Left == 30);
			Assert.IsTrue (widget.Right == 80);
			Assert.IsTrue (widget.Top == 60);
			Assert.IsTrue (widget.Bottom == 20);
			
			Point pt_test   = new Point (40, 35);
			Point pt_client = widget.MapParentToClient (pt_test);
			Point pt_widget = widget.MapClientToParent (pt_client);
			
			Assert.IsTrue (pt_client.X == 10);
			Assert.IsTrue (pt_client.Y == 15);
			Assert.IsTrue (pt_widget.X == pt_test.X);
			Assert.IsTrue (pt_widget.Y == pt_test.Y);
			
			widget.SetClientAngle (90);
			
			Assert.IsTrue (widget.Client.Angle == 90);
			Assert.IsTrue (widget.Client.Width == 40);
			Assert.IsTrue (widget.Client.Height == 50);
			
			widget.SetClientAngle (180);
			widget.SetClientZoom (0.5f);
			
			Assert.IsTrue (widget.Client.Angle == 180);
			Assert.IsTrue (widget.Client.Width == 100);
			Assert.IsTrue (widget.Client.Height == 80);
			
			widget.SetClientAngle (180);
			widget.SetClientZoom (0.5f);
			
			pt_test   = new Point (widget.Left, widget.Bottom);
			pt_client = widget.MapParentToClient (pt_test);
			pt_widget = widget.MapClientToParent (pt_client);
			
			Assert.IsTrue (Transform.Equal (pt_widget, pt_test));
			Assert.IsTrue (Transform.Equal (pt_client.X, widget.Client.Width));
			Assert.IsTrue (Transform.Equal (pt_client.Y, widget.Client.Height));
			
			
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
			
				Assert.IsTrue (Transform.Equal (pt_widget, pt_test));
				
				switch (angle)
				{
					case 0:
						Assert.IsTrue (Transform.Equal (pt_client.X, ox / zoom), "0° failed");
						Assert.IsTrue (Transform.Equal (pt_client.Y, oy / zoom), "0° failed");
						break;
					
					case 90:
						Assert.IsTrue (Transform.Equal (pt_client.X, oy / zoom), "90° failed");
						Assert.IsTrue (Transform.Equal (pt_client.Y, widget.Client.Height - ox / zoom), "90° failed");
						break;
					
					case 180:
						Assert.IsTrue (Transform.Equal (pt_client.X, (widget.Client.Width - ox / zoom)), "180° failed");
						Assert.IsTrue (Transform.Equal (pt_client.Y, (widget.Client.Height - oy / zoom)), "180° failed");
						break;
					
					case 270:
						Assert.IsTrue (Transform.Equal (pt_client.X, (widget.Client.Width - oy / zoom)), "270° failed");
						Assert.IsTrue (Transform.Equal (pt_client.Y, ox / zoom), "270° failed");
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
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientAngle (180);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			widget.SetClientAngle (270);
			transform = widget.GetTransformToClient ();
			
			pt2 = widget.MapParentToClient (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
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
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (7);
			widget.SetClientAngle (180);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (1.5f);
			widget.SetClientAngle (270);
			
			rect2 = widget.MapParentToClient (rect1);
			pt3   = widget.MapParentToClient (pt1);
			pt4   = widget.MapParentToClient (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
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
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (7);
			widget.SetClientAngle (180);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
			
			widget.SetClientZoom (1.5f);
			widget.SetClientAngle (270);
			
			rect2 = widget.MapClientToParent (rect1);
			pt3   = widget.MapClientToParent (pt1);
			pt4   = widget.MapClientToParent (pt2);
			
			System.Console.Out.WriteLine ("rect = " + rect2.ToString ());
			System.Console.Out.WriteLine ("pts  = " + pt3.ToString () + " / " + pt4.ToString ());
			
			Assert.IsTrue (Transform.Equal (rect2.Left,   System.Math.Min (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Bottom, System.Math.Min (pt3.Y, pt4.Y)));
			Assert.IsTrue (Transform.Equal (rect2.Right,  System.Math.Max (pt3.X, pt4.X)));
			Assert.IsTrue (Transform.Equal (rect2.Top,    System.Math.Max (pt3.Y, pt4.Y)));
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
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientZoom (3);
			widget.SetClientAngle (90);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (180);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
			
			transform.Reset ();
			widget.SetClientAngle (270);
			transform = widget.GetTransformToParent ();
			
			pt2 = widget.MapClientToParent (pt1);
			pt3 = transform.TransformDirect (pt1);
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Equal (pt2, pt3));
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
			
			Assert.IsTrue (identity.Equals (transform));
			
			transform = widget.GetTransformToParent ();
			transform.MultiplyBy (widget.GetTransformToClient ());
			
			Assert.IsTrue (identity.Equals (transform));
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
			
			Assert.IsTrue (Epsitec.Common.Drawing.Transform.Multiply (t1, t2).Equals (new Epsitec.Common.Drawing.Transform ()));
		}
		
		[Test] public void CheckDocking()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckDocking";
			window.Root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
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
			
			Assert.AreEqual ("XA", w1.FullPathName);
			Assert.AreEqual ("XA.XB", w2.FullPathName);
			Assert.AreEqual ("XA.XB.XC", w3.FullPathName);
			
			Assert.AreEqual (w2, w1.FindChildByPath ("XA.XB"));
			Assert.AreEqual (w3, w1.FindChildByPath ("XA.XB.XC"));
			
			Widget[] find = Widget.FindAllFullPathWidgets (Support.RegexFactory.FromSimpleJoker ("*XB*"));
			
			Assert.AreEqual (2, find.Length);
			Assert.AreEqual (w2, find[0]);
			Assert.AreEqual (w3, find[1]);
		}
		
		[Test] public void CheckCommandName()
		{
			Widget w1 = new Widget (); w1.Name = "A"; w1.Command = "a";
			Widget w2 = new Widget (); w2.Name = "B"; w2.Command = "b ()";  w2.Parent = w1;
			Widget w3 = new Widget (); w3.Name = "C"; w3.Command = "c (1)"; w3.Parent = w2;
			
			Assert.AreEqual ("a",     w1.Command);
			Assert.AreEqual ("b ()",  w2.Command);
			Assert.AreEqual ("c (1)", w3.Command);
			
			Assert.AreEqual ("a", w1.CommandName);
			Assert.AreEqual ("b", w2.CommandName);
			Assert.AreEqual ("c", w3.CommandName);
			
			Assert.IsTrue (w1.IsCommand);
			Assert.IsTrue (w2.IsCommand);
			Assert.IsTrue (w3.IsCommand);
			
			Assert.AreEqual (w2, w1.FindChildByPath ("A.B"));
			Assert.AreEqual (w3, w1.FindChildByPath ("A.B.C"));
			
			Widget[] find = w1.FindCommandWidgets ("b");
			
			Assert.AreEqual (1, find.Length);
			Assert.AreEqual (w2, find[0]);
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
			Widget[]     find  = Widget.FindAllCommandWidgets ("save");
			CommandState state = find.Length == 0 ? null : find[0].CommandState;
			
			Assert.IsNotNull (state);
			
			for (int i = 0; i < find.Length; i++)
			{
				find[i].SetEnabled (true);
				find[i].Invalidate ();
				
				System.Diagnostics.Debug.Assert (find[i].CommandState == state);
			}
		}
		
		[Test] public void CheckSaveCommandDisable()
		{
			Widget[] find = Widget.FindAllCommandWidgets ("save");
			
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
		
		
		static CommandState open_state = new CommandState ("open");
		
		[Test] public void CheckFindChildBasedOnName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.Name = "a";	w1.Parent = root;
			Widget w2 = new Widget ();					w2.Parent = root;
			Widget w3 = new Widget ();	w3.Name = "b";	w3.Parent = w2;
			
			Assert.AreEqual (w1, root.FindChild ("a"));
			Assert.AreEqual (w3, root.FindChild ("b"));
		}
		
		[Test] public void CheckFindChildBasedOnCommandName()
		{
			Widget root = new Widget ();
			Widget w1 = new Widget ();	w1.Command = "a";	w1.Parent = root;
			Widget w2 = new Widget ();						w2.Parent = root;
			Widget w3 = new Widget ();	w3.Command = "b";	w3.Parent = w2;
			Widget w4 = new Widget ();	w4.Command = "c";	w4.Parent = w2;
			Widget w5 = new Widget ();	w5.Command = "d";	w5.Parent = w4;
			Widget w6 = new Widget ();	w6.Name = "e";		w6.Parent = w1;
			
			Assert.AreEqual (w1, root.FindCommandWidgets ("a") [0]);
			Assert.AreEqual (w3, root.FindCommandWidgets ("b") [0]);
			Assert.AreEqual (w4, root.FindCommandWidgets ("c") [0]);
			Assert.AreEqual (w5, root.FindCommandWidgets ("d") [0]);
			
			Assert.IsTrue (root.FindCommandWidgets ("e").Length == 0);
			
			Widget[] command_widgets = root.FindCommandWidgets ();
			
			Assert.AreEqual (4, command_widgets.Length);
		}
		
		[Test] public void CheckColorSelector()
		{
			Window window = new Window ();
			window.Text = "CheckColorSelector";
			window.ClientSize = new Drawing.Size (250, 250);
			
			ColorSelector selector = new ColorSelector ();
			
			selector.Dock = DockStyle.Fill;
			selector.Parent = window.Root;
			
			window.Show ();
		}
		
		
		[Test] public void CheckSmartTagColor()
		{
			Window window = new Window ();
			window.Text = "CheckSmartTagColor";
			window.ClientSize = new Drawing.Size (510, 220);
			window.MakeFixedSizeWindow ();
			
			ColorSelector selector1, selector2;
			
			selector1 = new ColorSelector ();
			selector1.Bounds = new Drawing.Rectangle (100, 10, 200, 200);
			selector1.Parent = window.Root;
			selector1.Changed += new EventHandler (this.HandleSelectorChangedForeground);
			
			selector2 = new ColorSelector ();
			selector2.Bounds = new Drawing.Rectangle (300, 10, 200, 200);
			selector2.Parent = window.Root;
			selector2.Changed += new EventHandler (this.HandleSelectorChangedBackground);
			
			Tag tag;
			
			tag = new Tag ("", "tag1");
			tag.Bounds = new Drawing.Rectangle (10, 10, 10, 10);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag2");
			tag.Bounds = new Drawing.Rectangle (10, 25, 15, 15);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag3");
			tag.Bounds = new Drawing.Rectangle (10, 45, 20, 20);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag4");
			tag.Bounds = new Drawing.Rectangle (10, 70, 25, 25);
			tag.Parent = window.Root;
			
			selector1.Color = new RichColor(tag.Color);
			selector2.Color = new RichColor(tag.BackColor);
			
			window.Show ();
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
		
		private void HandleSelectorChangedForeground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color.Basic;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
		}
		
		private void HandleSelectorChangedBackground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color.Basic;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
		}
	}
}
