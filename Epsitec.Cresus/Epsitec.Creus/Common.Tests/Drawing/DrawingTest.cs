using NUnit.Framework;
using Epsitec.Common.Drawing;


namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class DrawingTest
	{
		public DrawingTest()
		{
		}
		
		[Test] public void CheckPixmap()
		{
			Pixmap pixmap = new Pixmap ();
			
			pixmap.Size = new System.Drawing.Size (200, 100);
			pixmap.Size = new System.Drawing.Size (100, 50);
			
			pixmap.Dispose ();
		}
		
		[Test] public void CheckPointToStringParse()
		{
			Point pt1 = new Point (10, 20);
			Point pt2 = Point.Parse ("10;20");
			Point pt3 = Point.Parse ("*;30", pt1);
			Point pt4 = Point.Parse ("40;*", pt1);
			
			Assert.AreEqual (pt1, pt2);
			Assert.AreEqual (new Point (10, 30), pt3);
			Assert.AreEqual (new Point (40, 20), pt4);
			Assert.AreEqual ("10;20", pt1.ToString ());
		}
		
		[Test] public void CheckSizeToStringParse()
		{
			Size sz1 = new Size (10, 20);
			Size sz2 = Size.Parse ("10;20");
			Size sz3 = Size.Parse ("*;30", sz1);
			Size sz4 = Size.Parse ("40;*", sz1);
			
			Assert.AreEqual (sz1, sz2);
			Assert.AreEqual (new Size (10, 30), sz3);
			Assert.AreEqual (new Size (40, 20), sz4);
			Assert.AreEqual ("10;20", sz1.ToString ());
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx1()
		{
			Common.Drawing.Graphics graphics = new Graphics ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient (graphics);
			gradient.SetColors (0, 0, 0, 0, 1, 1, 1, 1);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx2()
		{
			Common.Drawing.Graphics graphics = new Graphics ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient (graphics);
			gradient.SetParameters (0, 100);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx3()
		{
			Common.Drawing.Graphics graphics = new Graphics ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient (graphics);
			gradient.Fill = Common.Drawing.GradientFill.Conic;
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckRendererGradientEx4()
		{
			Common.Drawing.Graphics graphics = new Graphics ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient (graphics);
			gradient.SetColors (new double[100], new double[256], new double[256], new double[256]);
		}
		
		
		[Test] public void CheckRendererGradient()
		{
			Pixmap pixmap = new Pixmap ();
			Common.Drawing.Graphics graphics = new Graphics ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient (graphics);
			
			pixmap.Size = new System.Drawing.Size (200, 200);
			gradient.Pixmap = pixmap;
			gradient.Fill = Common.Drawing.GradientFill.Circle;
			gradient.SetColors (Color.FromBrightness (0.0), Color.FromBrightness (1.0));
			gradient.SetParameters (0, 100);
			
			gradient.Dispose ();
			pixmap.Dispose ();
		}
	}
}
