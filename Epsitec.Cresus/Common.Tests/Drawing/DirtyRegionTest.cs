using NUnit.Framework;
using Epsitec.Common.Drawing;


namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class DirtyRegionTest
	{
		public DirtyRegionTest()
		{
		}
		
		[Test] public void CheckAdd()
		{
			DirtyRegion s = new DirtyRegion (2);
			
			Rectangle r1 = new Rectangle (1, 1, 2, 4);
			Rectangle r2 = new Rectangle (4, 2, 2, 2);
			Rectangle r3 = new Rectangle (2, 6, 2, 1);
			Rectangle r4 = new Rectangle (2, 3, 1, 3);
			Rectangle r5 = new Rectangle (1, 0, 3, 7);
			
			System.Console.WriteLine ("r1 = {0}", r1.ToString ());
			System.Console.WriteLine ("r2 = {0}", r2.ToString ());
			System.Console.WriteLine ("r3 = {0}", r3.ToString ());
			System.Console.WriteLine ("r4 = {0}", r4.ToString ());
			System.Console.WriteLine ("r5 = {0}", r5.ToString ());
			
			s.Add (r1);
			s.Add (r2);
			
			Rectangle[] rects = s.Rectangles;
			
			Assert.AreEqual (2, rects.Length);
			Assert.AreEqual (r1, rects[0]);
			Assert.AreEqual (r2, rects[1]);
			
			s.Add (r3);
			
			rects = s.Rectangles;
			
			System.Console.WriteLine ("rects[0] = {0}", rects[0].ToString ());
			System.Console.WriteLine ("rects[1] = {0}", rects[1].ToString ());
			
			Assert.AreEqual (2, rects.Length);
			Assert.AreEqual (r2, rects[0]);
			Assert.AreEqual (Rectangle.Union (r1, r3), rects[1]);
			
			s.Add (r4);
			
			rects = s.Rectangles;
			
			System.Console.WriteLine ("rects[0] = {0}", rects[0].ToString ());
			System.Console.WriteLine ("rects[1] = {0}", rects[1].ToString ());
			
			Assert.AreEqual (2, rects.Length);
			Assert.AreEqual (r2, rects[0]);
			Assert.AreEqual (Rectangle.Union (r1, r3), rects[1]);
			
			s.Add (r5);
			
			rects = s.Rectangles;
			
			System.Console.WriteLine ("rects[0] = {0}", rects[0].ToString ());
			System.Console.WriteLine ("rects[1] = {0}", rects[1].ToString ());
			
			Assert.AreEqual (2, rects.Length);
			Assert.AreEqual (r2, rects[0]);
			Assert.AreEqual (r5, rects[1]);
		}
		
		[Test] public void CheckGenerateStrips()
		{
			DirtyRegion s = new DirtyRegion ();
			
			Rectangle r1 = new Rectangle (1, 1, 2, 4);
			Rectangle r2 = new Rectangle (4, 2, 2, 2);
			Rectangle r3 = new Rectangle (2, 6, 2, 1);
			Rectangle r4 = new Rectangle (2, 3, 2, 1);
			
			System.Console.WriteLine ("r1 = {0}", r1.ToString ());
			System.Console.WriteLine ("r2 = {0}", r2.ToString ());
			System.Console.WriteLine ("r3 = {0}", r3.ToString ());
			System.Console.WriteLine ("r4 = {0}", r4.ToString ());
			
			s.Add (r1);
			s.Add (r2);
			s.Add (r3);
			
			Rectangle[] rects = s.Rectangles;
			
			Assert.AreEqual (3, rects.Length);
			Assert.AreEqual (r1, rects[0]);
			Assert.AreEqual (r2, rects[1]);
			Assert.AreEqual (r3, rects[2]);
			
			Rectangle[] strips = s.GenerateStrips ();
			
			for (int i = 0; i < strips.Length; i++)
			{
				System.Console.WriteLine ("Strip {0}: {1}", i, strips[i].ToString ());
			}
			
			Assert.AreEqual (5, strips.Length);
			
			s.Add (r4);
			
			strips = s.GenerateStrips ();
			
			for (int i = 0; i < strips.Length; i++)
			{
				System.Console.WriteLine ("Strip {0}: {1}", i, strips[i].ToString ());
			}
			
			Assert.AreEqual (6, strips.Length);
		}
	}
}
