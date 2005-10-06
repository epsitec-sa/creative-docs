using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class VisualTest
	{
		[SetUp] public void Initialise()
		{
		}

		
		[Test] public void CheckVisualEnable()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();
			
			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);
			
			a.IsEnabledChanged += new Epsitec.Common.Types.PropertyChangedEventHandler(VisualTest.A_IsEnabledChanged);
			b.IsEnabledChanged += new Epsitec.Common.Types.PropertyChangedEventHandler(VisualTest.B_IsEnabledChanged);
			c1.IsEnabledChanged += new Epsitec.Common.Types.PropertyChangedEventHandler(VisualTest.C1_IsEnabledChanged);
			c2.IsEnabledChanged += new Epsitec.Common.Types.PropertyChangedEventHandler(VisualTest.C2_IsEnabledChanged);
			
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
		
		
		private static System.Text.StringBuilder buffer;
		
		private static void A_IsEnabledChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			buffer.Append (" A:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void B_IsEnabledChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			buffer.Append (" B:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void C1_IsEnabledChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			buffer.Append (" C1:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
		
		private static void C2_IsEnabledChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			buffer.Append (" C2:");
			buffer.Append ((bool) e.OldValue == true ? "1" : "0");
			buffer.Append ("->");
			buffer.Append ((bool) e.NewValue == true ? "1" : "0");
		}
	}
}