//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;

namespace Epsitec.Common.Identity
{
	[TestFixture]
	public class IdentityCardTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}


		[Test]
		public void CheckSerialization()
		{
			string path = @"S:\Epsitec.Cresus\Common.Identity\Identities";

			IdentityCard card = new IdentityCard ();

			card.DeveloperId = 123;
			card.RawImage = System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "PA.png"));
			card.UserName = "Jean Dupont";

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (card);

			System.Console.Out.WriteLine (xml);

			IdentityCard restoredCard = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityCard;

			Assert.AreEqual (card.DeveloperId, restoredCard.DeveloperId);
			Assert.AreEqual (card.RawImage, restoredCard.RawImage);
			Assert.AreEqual (card.UserName, restoredCard.UserName);
		}

		[Test]
		public void CheckUserInterface()
		{
			Widgets.Window window = new Epsitec.Common.Widgets.Window ();
			UI.IdentityCardWidget widget = new UI.IdentityCardWidget (window.Root);
			widget.IdentityCard = IdentityRepository.Default.FindIdentityCard ("Pierre Arnaud");
			widget.Dock = Widgets.DockStyle.Fill;
			widget.Margins = new Drawing.Margins (4, 4, 4, 4);
			window.Show ();
			Widgets.Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CreateEpsitecIdentities()
		{
			IdentityRepository repository = new IdentityRepository ();

			string path = @"S:\Epsitec.Cresus\Common.Identity\Identities";

			IdentityCard cardChristian = new IdentityCard ("Christian Alleyn", 5, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "CA.png")));
			IdentityCard cardPierre    = new IdentityCard ("Pierre Arnaud",    1, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "PA.png")));
			IdentityCard cardDavid     = new IdentityCard ("David Besuchet",   3, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "DB.png")));
			IdentityCard cardYves      = new IdentityCard ("Yves Raboud",      4, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "YR.png")));
			IdentityCard cardDaniel    = new IdentityCard ("Daniel Roux",      2, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "DR.png")));

			repository.IdentityCards.Add (cardChristian);	//	Alleyn
			repository.IdentityCards.Add (cardPierre);		//	Arnaud
			repository.IdentityCards.Add (cardDavid);		//	Besuchet
			repository.IdentityCards.Add (cardYves);		//	Raboud
			repository.IdentityCards.Add (cardDaniel);		//	Roux

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (repository);

			System.IO.File.WriteAllText (System.IO.Path.Combine (path, IdentityRepository.DefaultIdentitiesFileName), xml);

			IdentityRepository restoredRepository = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityRepository;
		}
	}
}
