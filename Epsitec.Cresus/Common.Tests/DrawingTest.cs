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

		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx1()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetColors (0, 0, 0, 0, 1, 1, 1, 1);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx2()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetParameters (0, 100);
		}
		
		[Test] [ExpectedException (typeof (System.NullReferenceException))]
		public void CheckRendererGradientEx3()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.Fill = Common.Drawing.GradientFill.Conic;
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckRendererGradientEx4()
		{
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			gradient.SetColors (new double[100], new double[256], new double[256], new double[256]);
		}
		
		
		[Test] public void CheckRendererGradient()
		{
			Pixmap pixmap = new Pixmap ();
			Common.Drawing.Renderer.Gradient gradient = new Common.Drawing.Renderer.Gradient ();
			
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
