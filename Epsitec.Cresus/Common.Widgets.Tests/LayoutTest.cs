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

			context.ExecuteMeasure ();
			
			Assert.AreEqual (0, context.MeasureQueueLength);
			
			Assert.IsTrue (c1.ContainsLocalValue (Layouts.LayoutMeasure.WidthProperty));
			Assert.IsTrue (c1.ContainsLocalValue (Layouts.LayoutMeasure.HeightProperty));
		}
	}
}