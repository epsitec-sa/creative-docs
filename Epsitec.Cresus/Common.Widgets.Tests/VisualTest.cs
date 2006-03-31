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
		public void CheckStruct()
		{
			XS xs = new XS ();
			Assert.AreEqual (0, xs.Count);
			xs.Foo ();
			Assert.AreEqual (1, xs.Count);

			//	Travailler avec une interface pointant sur une structure cr�e
			//	en fait une version "boxed" de la structure, ce qui signifie
			//	que l'on travaille sur une copie, plus sur un original...
			
			IX ix = xs;

			ix.Foo ();
			Assert.AreEqual (1, xs.Count);
		}

		interface IX
		{
			void Foo();
		}
		struct XS : IX
		{
			public int Count
			{
				get
				{
					return this.count;
				}
			}
			#region IX Members

			public void Foo()
			{
				this.count++;
			}

			#endregion
			int count;
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

			a.IsEnabledChanged += new PropertyChangedEventHandler (VisualTest.A_IsEnabledChanged);
			b.IsEnabledChanged += new PropertyChangedEventHandler (VisualTest.B_IsEnabledChanged);
			c1.IsEnabledChanged += new PropertyChangedEventHandler (VisualTest.C1_IsEnabledChanged);
			c2.IsEnabledChanged += new PropertyChangedEventHandler (VisualTest.C2_IsEnabledChanged);

			Assert.IsTrue (a.Enable);
			Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);
			Assert.IsTrue (b.IsEnabled);
			Assert.IsTrue (c1.Enable);
			Assert.IsTrue (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsTrue (c2.IsEnabled);

			VisualTest.buffer = new System.Text.StringBuilder ();
			a.Enable = false;

			Assert.IsFalse (a.Enable);
			Assert.IsFalse (a.IsEnabled);
			Assert.IsTrue (b.Enable);
			Assert.IsFalse (b.IsEnabled);
			Assert.IsTrue (c1.Enable);
			Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual (" A:1->0 B:1->0 C1:1->0 C2:1->0", VisualTest.buffer.ToString ());

			VisualTest.buffer = new System.Text.StringBuilder ();
			a.Enable = true;

			Assert.IsTrue (a.Enable);
			Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);
			Assert.IsTrue (b.IsEnabled);
			Assert.IsTrue (c1.Enable);
			Assert.IsTrue (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsTrue (c2.IsEnabled);
			Assert.AreEqual (" A:0->1 B:0->1 C1:0->1 C2:0->1", VisualTest.buffer.ToString ());

			VisualTest.buffer = new System.Text.StringBuilder ();
			b.Enable = false;

			Assert.IsTrue (a.Enable);
			Assert.IsTrue (a.IsEnabled);
			Assert.IsFalse (b.Enable);
			Assert.IsFalse (b.IsEnabled);
			Assert.IsTrue (c1.Enable);
			Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual (" B:1->0 C1:1->0 C2:1->0", VisualTest.buffer.ToString ());

			VisualTest.buffer = new System.Text.StringBuilder ();
			c1.Enable = false;

			Assert.IsTrue (a.Enable);
			Assert.IsTrue (a.IsEnabled);
			Assert.IsFalse (b.Enable);
			Assert.IsFalse (b.IsEnabled);
			Assert.IsFalse (c1.Enable);
			Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsFalse (c2.IsEnabled);
			Assert.AreEqual ("", VisualTest.buffer.ToString ());

			VisualTest.buffer = new System.Text.StringBuilder ();
			b.Enable = true;

			Assert.IsTrue (a.Enable);
			Assert.IsTrue (a.IsEnabled);
			Assert.IsTrue (b.Enable);
			Assert.IsTrue (b.IsEnabled);
			Assert.IsFalse (c1.Enable);
			Assert.IsFalse (c1.IsEnabled);
			Assert.IsTrue (c2.Enable);
			Assert.IsTrue (c2.IsEnabled);
			Assert.AreEqual (" B:0->1 C2:0->1", VisualTest.buffer.ToString ());

			Assert.AreEqual (a, b.Parent);

			a.Children.Clear ();
		}

		[Test]
		public void CheckVisualVisibility()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();
			
			EventHandlerSupport handler = new EventHandlerSupport ();

			a.Name = "a";
			b.Name = "b";
			c1.Name = "c1";
			c2.Name = "c2";
			
			//	Triche pour que 'a' se comporte comme WindowRoot dans une
			//	fen�tre visible; sans cela, 'a' resterait tout le temps
			//	invisible :

			a.SetValueBase (Visual.IsVisibleProperty, true);

			a.IsVisibleChanged += handler.RecordEventAndName;
			b.IsVisibleChanged += handler.RecordEventAndName;
			c1.IsVisibleChanged += handler.RecordEventAndName;
			c2.IsVisibleChanged += handler.RecordEventAndName;

			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);

			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsTrue (b.Visibility);
			Assert.IsTrue (b.IsVisible);
			Assert.IsTrue (c1.Visibility);
			Assert.IsTrue (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsTrue (c2.IsVisible);

			System.Console.Out.WriteLine (handler.Log);
			handler.Clear ();
			
			a.Visibility = false;

			Assert.IsFalse (a.Visibility);
			Assert.IsFalse (a.IsVisible);
			Assert.IsTrue (b.Visibility);
			Assert.IsFalse (b.IsVisible);
			Assert.IsTrue (c1.Visibility);
			Assert.IsFalse (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsFalse (c2.IsVisible);
			Assert.AreEqual ("a-IsVisible:True,False.b-IsVisible:True,False.c1-IsVisible:True,False.c2-IsVisible:True,False.", handler.Log);
			
			handler.Clear ();
			a.Visibility = true;
			
			Assert.IsTrue (a.Visibility);
			Assert.IsFalse (a.IsVisible);
			
			//	Re-triche (cf. plus haut) :
			
			a.SetValueBase (Visual.IsVisibleProperty, true);

			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsTrue (b.Visibility);
			Assert.IsTrue (b.IsVisible);
			Assert.IsTrue (c1.Visibility);
			Assert.IsTrue (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsTrue (c2.IsVisible);
			Assert.AreEqual ("a-IsVisible:False,True.b-IsVisible:False,True.c1-IsVisible:False,True.c2-IsVisible:False,True.", handler.Log);

			handler.Clear ();
			b.Visibility = false;

			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsFalse (b.Visibility);
			Assert.IsFalse (b.IsVisible);
			Assert.IsTrue (c1.Visibility);
			Assert.IsFalse (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsFalse (c2.IsVisible);
			Assert.AreEqual ("b-IsVisible:True,False.c1-IsVisible:True,False.c2-IsVisible:True,False.", handler.Log);

			handler.Clear ();
			c1.Visibility = false;

			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsFalse (b.Visibility);
			Assert.IsFalse (b.IsVisible);
			Assert.IsFalse (c1.Visibility);
			Assert.IsFalse (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsFalse (c2.IsVisible);
			Assert.AreEqual ("", handler.Log);

			handler.Clear ();
			b.Visibility = true;

			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsTrue (b.Visibility);
			Assert.IsTrue (b.IsVisible);
			Assert.IsFalse (c1.Visibility);
			Assert.IsFalse (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsTrue (c2.IsVisible);
			Assert.AreEqual ("b-IsVisible:False,True.c2-IsVisible:False,True.", handler.Log);

			Assert.AreEqual (a, b.Parent);
			
			handler.Clear ();
			a.Children.Clear ();
			
			Assert.IsTrue (a.Visibility);
			Assert.IsTrue (a.IsVisible);
			Assert.IsTrue (b.Visibility);
			Assert.IsFalse (b.IsVisible);
			Assert.IsFalse (c1.Visibility);
			Assert.IsFalse (c1.IsVisible);
			Assert.IsTrue (c2.Visibility);
			Assert.IsFalse (c2.IsVisible);
			Assert.AreEqual ("b-IsVisible:True,False.c2-IsVisible:True,False.", handler.Log);
		}

		[Test]
		public void CheckVisualChildren1()
		{
			Visual root = new Visual ();

			root.Name = "root";
			
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c = new Visual ();
			Visual x = new Visual ();

			a.Name = "a";
			b.Name = "b";
			c.Name = "c";
			x.Name = "x";
			
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

			Assert.AreEqual (null, root.Children.FindPrevious (b));
			Assert.AreEqual (b, root.Children.FindPrevious (a));
			Assert.AreEqual (a, root.Children.FindPrevious (c));
			Assert.AreEqual (null, root.Children.FindPrevious (x));

			Assert.AreEqual (a, root.Children.FindNext (b));
			Assert.AreEqual (c, root.Children.FindNext (a));
			Assert.AreEqual (null, root.Children.FindNext (c));
			Assert.AreEqual (null, root.Children.FindNext (x));
			
			a.Parent = null;

			Assert.AreEqual (2, root.Children.Count);
			Assert.AreEqual (null, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (root, c.Parent);
			Assert.AreEqual (b, root.Children[0]);
			Assert.AreEqual (c, root.Children[1]);

			root.Children.Clear ();

			Assert.IsFalse (root.HasChildren);
			
			Assert.AreEqual (null, a.Parent);
			Assert.AreEqual (null, b.Parent);
			Assert.AreEqual (null, c.Parent);

			root.Children.AddRange (new Visual[] { a, b, c });
			
			Assert.AreEqual (3, root.Children.Count);
			Assert.AreEqual (root, a.Parent);
			Assert.AreEqual (root, b.Parent);
			Assert.AreEqual (root, c.Parent);
			Assert.AreEqual (a, root.Children[0]);
			Assert.AreEqual (b, root.Children[1]);
			Assert.AreEqual (c, root.Children[2]);
		}
		
		[Test]
		public void CheckVisualChildren2()
		{
			Visual r1 = new Visual ();
			Visual r2 = new Visual ();
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();

			r1.Name = "r1";
			r2.Name = "r2";
			
			a.Name = "a";
			b.Name = "b";
			c1.Name = "c1";
			c2.Name = "c2";
			
			EventHandlerSupport handler = new EventHandlerSupport ();

			r1.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			r2.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			a.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			b.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			c1.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			c2.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);

			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);

			r1.Children.Add (a);

			r1.SetValue (Visual.WindowProperty, "W1");

			Assert.AreEqual ("r1-Window:<null>,W1.a-Window:<null>,W1.b-Window:<null>,W1.c1-Window:<null>,W1.c2-Window:<null>,W1.", handler.Log);
			handler.Clear ();

			Assert.AreEqual ("W1", a.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W1", b.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W1", c1.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W1", c2.GetValue (Visual.WindowProperty));

			r2.SetValue (Visual.WindowProperty, "W2");

			Assert.AreEqual ("r2-Window:<null>,W2.", handler.Log);
			handler.Clear ();
			
			r1.Children.Remove (a);

			Assert.AreEqual ("a-Window:W1,<null>.b-Window:W1,<null>.c1-Window:W1,<null>.c2-Window:W1,<null>.", handler.Log);
			handler.Clear ();
			
			Assert.IsNull (a.GetValue (Visual.WindowProperty));
			Assert.IsNull (b.GetValue (Visual.WindowProperty));
			Assert.IsNull (c1.GetValue (Visual.WindowProperty));
			Assert.IsNull (c2.GetValue (Visual.WindowProperty));

			r2.Children.Add (a);

			Assert.AreEqual ("a-Window:<null>,W2.b-Window:<null>,W2.c1-Window:<null>,W2.c2-Window:<null>,W2.", handler.Log);
			handler.Clear ();
			
			Assert.AreEqual ("W2", a.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W2", b.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W2", c1.GetValue (Visual.WindowProperty));
			Assert.AreEqual ("W2", c2.GetValue (Visual.WindowProperty));

			Visual c10 = new Visual ();
			Visual c11 = new Visual ();

			c10.Name = "c10";
			c11.Name = "c11";
			
			c10.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);
			c11.AddEventHandler (Visual.WindowProperty, handler.RecordEventAndName);

			c1.Children.Add (c10);
			c1.Children.Add (c11);

			Assert.AreEqual ("c10-Window:<null>,W2.c11-Window:<null>,W2.", handler.Log);
			handler.Clear ();

			c1.SetValue (Visual.WindowProperty, "WX");

			Assert.AreEqual ("WX", c1.GetValue (Visual.WindowProperty));
			
			Assert.AreEqual ("c1-Window:W2,WX.c10-Window:W2,WX.c11-Window:W2,WX.", handler.Log);
			handler.Clear ();

			r1.Children.Add (a);

			Assert.AreEqual ("a-Window:W2,W1.b-Window:W2,W1.c2-Window:W2,W1.", handler.Log);
			handler.Clear ();
		}

		[Test]
		public void CheckMemoryUse()
		{
			int size = 1000*1000;
			X[] array = new X[size];

			long before = System.GC.GetTotalMemory (true);

			for (int i = 0; i < size; i++)
			{
				array[i] = new X ();
			}
			
			long after = System.GC.GetTotalMemory (true);

			System.Console.Out.WriteLine ("Allocated {0} bytes for {1} objects, {2:0.0} byte/int", after-before, array.Length, (after-before)*1.0/array.Length);
		}

		class X : IZ
		{
			public Y y;

			#region IZ Members

			public void Foo()
			{
				y.Foo ();
			}

			#endregion
		}
		struct Y : IZ
		{
			public int v;
			public void Foo()
			{
			}
		}
		interface IZ
		{
			void Foo();
		}

		[Test]
		public void CheckVisualSpeed()
		{
			Visual root = new Visual ();
			Visual parent = root;

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();

			for (int i = 0; i < 1; i++)
			{
				StaticText text = new StaticText ();
				text.Text = "Default";
				parent.Children.Add (text);
				Visual child = new Visual ();
				parent.Children.Add (child);
				parent = child;
			}

			for (int j = 0; j < 100; j++)
			{
				root = new Visual ();
				parent = root;

				stopwatch.Reset ();
				stopwatch.Start ();

				for (int i = 0; i < 150; i++)
				{
					StaticText text = new StaticText ();
					text.Text = "Default";
					parent.Children.Add (text);
					Visual child = new Visual ();
					parent.Children.Add (child);
					parent = child;
				}

				stopwatch.Stop ();

				System.Console.WriteLine ("Created tree, top-down: {0:0} ms. {1} x ContainsKeyboardFocus", stopwatch.ElapsedMilliseconds, Helpers.VisualTree.ContainsKeyboardFocusCounter);
				Helpers.VisualTree.ContainsKeyboardFocusCounter = 0;
				System.Console.Out.Flush ();
			}
		}


		#region Class EventHandlerSupport
		private class EventHandlerSupport
		{
			public string Log
			{
				get
				{
					return this.buffer.ToString ();
				}
			}

			public void RecordEvent(object sender, Types.DependencyPropertyChangedEventArgs e)
			{
				this.buffer.Append (e.PropertyName);
				this.buffer.Append (":");
				this.buffer.Append (e.OldValue == null ? "<null>" : e.OldValue);
				this.buffer.Append (",");
				this.buffer.Append (e.NewValue == null ? "<null>" : e.NewValue);
				this.buffer.Append (".");
			}
			public void RecordEventAndName(object sender, Types.DependencyPropertyChangedEventArgs e)
			{
				this.buffer.Append (Types.DependencyObjectTree.GetName (sender as Types.DependencyObject));
				this.buffer.Append ("-");
				this.buffer.Append (e.PropertyName);
				this.buffer.Append (":");
				this.buffer.Append (e.OldValue == null ? "<null>" : e.OldValue);
				this.buffer.Append (",");
				this.buffer.Append (e.NewValue == null ? "<null>" : e.NewValue);
				this.buffer.Append (".");
			}
			public void Clear()
			{
				this.buffer.Length = 0;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
		}
		#endregion
		
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