using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class ImageProviderTest
	{
		[Test] public void CheckGetImage()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:images/open.png");
			Drawing.Image im2 = Support.ImageProvider.Default.GetImage ("file:images/open.icon");
			Drawing.Image im3 = Support.ImageProvider.Default.GetImage ("file:images/non-existing-image.png");
			
			Assertion.AssertNotNull (im1);
			Assertion.AssertNotNull (im2);
			Assertion.AssertNull (im3);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx1()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:../open.png");
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx2()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:/open.png");
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx3()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:C:/open.png");
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx4()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:\\open.png");
		}
	}
}
