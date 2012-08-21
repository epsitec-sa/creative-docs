using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
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
			Image im1 = ImageProvider.Instance.GetImage ("file:images/open.png", Resources.DefaultManager);
			Image im2 = ImageProvider.Instance.GetImage ("file:images/open.icon", Resources.DefaultManager);
			Image im3 = ImageProvider.Instance.GetImage ("file:images/non-existing-image.png", Resources.DefaultManager);

			Assert.IsNotNull (im1);
			Assert.IsNotNull (im2);
			Assert.IsNull (im3);
		}

		[Test]
		public void CheckGetImageNames()
		{
			List<string> names = new List<string> ();

			names.AddRange (ImageProvider.Instance.GetImageNames ("file", Resources.DefaultManager));

			Assert.AreEqual (@"file:Images\About.icon", names[0]);
			Assert.AreEqual (@"file:Images\Down.icon", names[1]);
			
			names.Clear ();
			names.AddRange (ImageProvider.Instance.GetImageNames ("manifest", Resources.DefaultManager));

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
			Image im1 = ImageProvider.Instance.GetImage ("file:../open.png", Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx2()
		{
			Image im1 = ImageProvider.Instance.GetImage ("file:/open.png", Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx3()
		{
			Image im1 = ImageProvider.Instance.GetImage ("file:C:/open.png", Resources.DefaultManager);
		}
		
		[Test] [ExpectedException (typeof (System.ArgumentException))] public void CheckGetImageEx4()
		{
			Image im1 = ImageProvider.Instance.GetImage ("file:\\open.png", Resources.DefaultManager);
		}
	}
}
