using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;
	
	[TestFixture] public class LayoutTest
	{
		[Test]
		public void CheckMeasure()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();

			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);

			Layouts.LayoutContext context = Helpers.VisualTree.GetLayoutContext (a);
			
			Assert.IsNotNull (context);
			Assert.AreEqual (0, context.MeasureQueueLength);
			
			context.StartNewLayoutPass ();

			Layouts.LayoutContext.AddToMeasureQueue (c1);
			
			Assert.AreEqual (1, context.MeasureQueueLength);

			Assert.IsFalse (a.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsFalse (a.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));
			Assert.IsFalse (b.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsFalse (b.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));
			Assert.IsFalse (c1.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsFalse (c1.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));
			Assert.IsFalse (c2.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsFalse (c2.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));
			
			Layouts.LayoutContext.AddToMeasureQueue (c1);

			Assert.AreEqual (1, context.MeasureQueueLength);

			context.ExecuteMeasure ();
			
			Assert.AreEqual (0, context.MeasureQueueLength);
			
			Assert.IsTrue (c1.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsTrue (c1.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));

			Layouts.LayoutMeasure dxMeasure = Layouts.LayoutMeasure.GetWidth (c1);
			Layouts.LayoutMeasure dyMeasure = Layouts.LayoutMeasure.GetHeight (c1);

			Assert.AreEqual (0, dxMeasure.Min);
			Assert.AreEqual (0, dyMeasure.Min);
			Assert.AreEqual (double.PositiveInfinity, dxMeasure.Max);
			Assert.AreEqual (double.PositiveInfinity, dyMeasure.Max);
			Assert.AreEqual (c1.PreferredWidth, dxMeasure.Desired);
			Assert.AreEqual (c1.PreferredHeight, dyMeasure.Desired);

			c1.MinWidth = 20;

			Assert.AreEqual (1, context.MeasureQueueLength);
			
			c1.MinHeight = 12;
			c1.MaxHeight = 30;

			Assert.AreEqual (1, context.MeasureQueueLength);
			
			context.ExecuteMeasure ();

			Assert.AreEqual (dxMeasure, Layouts.LayoutMeasure.GetWidth (c1));
			Assert.AreEqual (dyMeasure, Layouts.LayoutMeasure.GetHeight (c1));

			Assert.AreEqual (20, dxMeasure.Min);
			Assert.AreEqual (double.PositiveInfinity, dxMeasure.Max);
			Assert.AreEqual (12, dyMeasure.Min);
			Assert.AreEqual (30, dyMeasure.Max);

			//	Au sein d'une m�me passe de layout, un minimum ne peut que cro�tre
			//	et un maximum que diminuer.
			
			c1.MinWidth = 15;
			c1.MaxHeight = 40;
			
			context.ExecuteMeasure ();
			
			Assert.AreEqual (20, dxMeasure.Min);
			Assert.AreEqual (30, dyMeasure.Max);
			
			context.StartNewLayoutPass ();

			c1.MinWidth = 10;
			c1.MaxHeight = 50;
			
			c2.MinWidth = 40;

			Assert.AreEqual (2, context.MeasureQueueLength);

			context.ExecuteMeasure ();

			Assert.AreEqual (10, dxMeasure.Min);
			Assert.AreEqual (50, dyMeasure.Max);
		}
	}
}