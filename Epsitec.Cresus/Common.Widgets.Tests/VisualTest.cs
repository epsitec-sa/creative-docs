using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;
	
	[TestFixture] public class VisualTest
	{
		[SetUp] public void Initialise()
		{
		}

		
		[Test]
		public void CheckVisualEnable()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();
			
			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);
			
			a.IsEnabledChanged += new PropertyChangedEventHandler(VisualTest.A_IsEnabledChanged);
			b.IsEnabledChanged += new PropertyChangedEventHandler(VisualTest.B_IsEnabledChanged);
			c1.IsEnabledChanged += new PropertyChangedEventHandler(VisualTest.C1_IsEnabledChanged);
			c2.IsEnabledChanged += new PropertyChangedEventHandler(VisualTest.C2_IsEnabledChanged);
			
			Assert.IsTrue (a.Enable);	Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);	Assert.IsTrue (b.IsEnabled);
			Assert.IsTrue (c1.Enable);	Assert.IsTrue (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsTrue (c2.IsEnabled);
			
			VisualTest.buffer = new System.Text.StringBuilder ();
			a.Enable = false;
			
			Assert.IsFalse (a.Enable);	Assert.IsFalse (a.IsEnabled);
			Assert.IsTrue (b.Enable);	Assert.IsFalse (b.IsEnabled);
			Assert.IsTrue (c1.Enable);	Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual (" A:1->0 B:1->0 C1:1->0 C2:1->0", VisualTest.buffer.ToString ());
			
			VisualTest.buffer = new System.Text.StringBuilder ();
			a.Enable = true;
			
			Assert.IsTrue (a.Enable);	Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);	Assert.IsTrue (b.IsEnabled);
			Assert.IsTrue (c1.Enable);	Assert.IsTrue (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsTrue (c2.IsEnabled);
			Assert.AreEqual (" A:0->1 B:0->1 C1:0->1 C2:0->1", VisualTest.buffer.ToString ());
			
			VisualTest.buffer = new System.Text.StringBuilder ();
			b.Enable = false;
			
			Assert.IsTrue (a.Enable);	Assert.IsTrue (a.IsEnabled);
			Assert.IsFalse (b.Enable);	Assert.IsFalse (b.IsEnabled);
			Assert.IsTrue (c1.Enable);	Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual (" B:1->0 C1:1->0 C2:1->0", VisualTest.buffer.ToString ());
			
			VisualTest.buffer = new System.Text.StringBuilder ();
			c1.Enable = false;
			
			Assert.IsTrue (a.Enable);	Assert.IsTrue (a.IsEnabled);
			Assert.IsFalse (b.Enable);	Assert.IsFalse (b.IsEnabled);
			Assert.IsFalse (c1.Enable);	Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual ("", VisualTest.buffer.ToString ());
			
			VisualTest.buffer = new System.Text.StringBuilder ();
			b.Enable = true;
			
			Assert.IsTrue (a.Enable);	Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);	Assert.IsTrue (b.IsEnabled);
			Assert.IsFalse (c1.Enable);	Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);	Assert.IsTrue (c2.IsEnabled);
			Assert.AreEqual (" B:0->1 C2:0->1", VisualTest.buffer.ToString ());
		}
		
		[Test]
		public void CheckVisualChildren()
		{
			Visual root = new Visual ();

			root.Name = "root";
			
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c = new Visual ();

			a.Name = "a";
			b.Name = "b";
			c.Name = "c";
			
			Assert.IsFalse (root.HasChildren);
			Assert.IsFalse (a.HasChildren);

			Assert.AreEqual (0, a.Children.Count);
			Assert.IsFalse (a.HasChildren);

			Collections.FlatChildrenCollection colA = a.Children;

			Assert.AreEqual (colA, a.Children);

			root.Children.Insert (0, a);

			Assert.IsTrue (root.HasChildren);
			Assert.AreEqual (1, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (a, root.Children[0]);
			
			root.Children.Insert (0, b);
			
			Assert.IsTrue (root.HasChildren);
			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (a, root.Children[1]);

			Assert.AreEqual (1, root.Children.IndexOf (a));
			Assert.AreEqual (0, root.Children.IndexOf (b));

			root.Children[1] = c;

			Assert.AreEqual (0, root.Children.IndexOf (b));
			Assert.AreEqual (1, root.Children.IndexOf (c));
			
			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (null, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (root, c.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (c, root.Children[1]);

			root.Children.Add (a);

			Assert.AreEqual (3, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (2, root.Children.IndexOf (a));

			Visual[] array = new Visual[3];
			root.Children.CopyTo (array, 0);

			Assert.AreEqual (b, array[0]);
			Assert.AreEqual (c, array[1]);
			Assert.AreEqual (a, array[2]);

			root.Children.Remove (c);
			
			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (null, c.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (a, root.Children[1]);

			a.Parent = root;
			c.Parent = root;

			Assert.AreEqual (3, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (root, c.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (a, root.Children[1]);
			Assert.AreEqual (c, root.Children[2]);
			
			a.Parent = null;

			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (null, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (root, c.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (c, root.Children[1]);
		}
		
		
		private static System.Text.StringBuilder buffer;
		
		private static void A_IsEnabledChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			buffer.Append (" A:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void B_IsEnabledChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			buffer.Append (" B:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void C1_IsEnabledChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			buffer.Append (" C1:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void C2_IsEnabledChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			buffer.Append (" C2:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
	}
}