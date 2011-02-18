using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture] public class ImageProviderTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Widget.Initialize ();
		}

		[Test]
		public void CheckGetImage()
		{
			Drawing.Image im1 = Support.ImageProvider.Default.GetImage ("file:images/open.png", Support.Resources.DefaultManager);
			Drawing.Image im2 = Support.ImageProvider.Default.GetImage ("file:images/open.icon", Support.Resources.DefaultManager);
			Drawing.Image im3 = Support.ImageProvider.Default.GetImage ("file:images/non-existing-image.png", Support.Resources.DefaultManager);

			Assert.IsNotNull (im1);
			Assert.IsNotNull (im2);
			Assert.IsNull (im3);
		}

		[Test]
		public void CheckGetImageNames()
		{
			List<string> names = new List<string> ();

			names.AddRange (Support.ImageProvider.Default.GetImageNames ("file", Support.Resources.DefaultManager));

			Assert.AreEqual (@"file:About.icon", names[0]);
			Assert.AreEqual (@"file:Down.icon", names[1]);
			
			names.Clear ();
			names.AddRange (Support.ImageProvider.Default.GetImageNames ("manifest", Support.Resources.DefaultManager));

			Assert.AreEqual (@"manifest:Epsitec.Common.Dialogs.Images.FavoritesAdd.icon", names[0]);
		}

		[Test]
		public void CheckGetManifestResourceNames()
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
