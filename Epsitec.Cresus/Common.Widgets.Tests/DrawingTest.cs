using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class Drawing
	{
		public Drawing()
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
			Point pt5 = Point.Parse (pt1.ToString ());
			
			Assertion.AssertEquals (pt1, pt2);
			Assertion.AssertEquals (new Point (10, 30), pt3);
			Assertion.AssertEquals (new Point (40, 20), pt4);
			Assertion.AssertEquals (pt1, pt5);
			Assertion.AssertEquals ("10;20", pt1.ToString ());
		}
		
		[Test] public void CheckSizeToStringParse()
		{
			Size sz1 = new Size (10, 20);
			Size sz2 = Size.Parse ("10;20");
			Size sz3 = Size.Parse ("*;30", sz1);
			Size sz4 = Size.Parse ("40;*", sz1);
			Size sz5 = Size.Parse (sz1.ToString ());
			
			Assertion.AssertEquals (sz1, sz2);
			Assertion.AssertEquals (new Size (10, 30), sz3);
			Assertion.AssertEquals (new Size (40, 20), sz4);
			Assertion.AssertEquals (sz1, sz5);
			Assertion.AssertEquals ("10;20", sz1.ToString ());
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx1()
		{
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient ();
			gradient.SetColors (0, 0, 0, 0, 1, 1, 1, 1);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx2()
		{
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient ();
			gradient.SetParameters (0, 100);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx3()
		{
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient ();
			gradient.Fill = Common.Drawing.GradientFill.Conic;
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckRendererGradientEx4()
		{
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient ();
			gradient.SetColors (new double[100], new double[256], new double[256], new double[256]);
		}
		
		
		[Test] public void CheckRendererGradient()
		{
			Pixmap pixmap = new Pixmap ();
			Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient ();
			
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
