using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class ImageProviderTest
	{
		[SetUp] public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			Epsitec.Common.Widgets.Widget.Initialise ();
		}
		
		[Test] public void CheckGetImage()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:images/open.png", Support.Resources.DefaultManager);
			Drawing.Image im2 = Support.ImageProvider.Default.GetImage ("file:images/open.icon", Support.Resources.DefaultManager);
			Drawing.Image im3 = Support.ImageProvider.Default.GetImage ("file:images/non-existing-image.png", Support.Resources.DefaultManager);
			
			Assert.IsNotNull (im1);
			Assert.IsNotNull (im2);
			Assert.IsNull (im3);
		}
		
		[Test] public void CheckGetManifestResourceNames()
		{
			System.Text.RegularExpressions.Regex regex = RegexFactory.FromSimpleJoker ("*.icon", RegexFactory.Options.IgnoreCase);
			string[] names = ImageProvider.GetManifestResourceNames (regex);
			
			int i = 0;
			
			foreach (string name in names)
			{
				System.Console.Out.WriteLine ("{0}: {1}", ++i, name);
			}
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx1()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:../open.png", Support.Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx2()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:/open.png", Support.Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx3()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:C:/open.png", Support.Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx4()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:\\open.png", Support.Resources.DefaultManager);
		}
	}
}
