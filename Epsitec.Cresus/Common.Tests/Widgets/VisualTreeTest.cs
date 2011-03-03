using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;
	using Epsitec.Common.Widgets;
	using Epsitec.Common.Widgets.Helpers;
	using Epsitec.Common.Widgets.Layouts;
	
	[TestFixture] public class VisualTreeTest
	{
		[Test]
		public void CheckGetRoot()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();

			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);

			Assert.AreEqual (a, VisualTree.GetRoot (a));
			Assert.AreEqual (a, VisualTree.GetRoot (b));
			Assert.AreEqual (a, VisualTree.GetRoot (c1));
			Assert.AreEqual (a, VisualTree.GetRoot (c2));
		}
		
		[Test]
		public void CheckGetLayoutContext()
		{
			Visual a = new Visual ();
			Visual b = new Visual ();
			Visual c1 = new Visual ();
			Visual c2 = new Visual ();

			a.Children.Add (b);
			b.Children.Add (c1);
			b.Children.Add (c2);

			int depth;
			LayoutContext context;

			context = VisualTree.GetLayoutContext (null, out depth);

			Assert.IsNull (context);
			Assert.AreEqual (0, depth);

			context = VisualTree.GetLayoutContext (a, out depth);

			Assert.IsNotNull (context);
			Assert.AreEqual (1, depth);

			Assert.AreEqual (context, VisualTree.GetLayoutContext (b, out depth));
			Assert.AreEqual (2, depth);

			Assert.AreEqual (context, VisualTree.GetLayoutContext (c1, out depth));
			Assert.AreEqual (3, depth);

			Assert.AreEqual (context, VisualTree.GetLayoutContext (c2, out depth));
			Assert.AreEqual (3, depth);
		}
	}
}