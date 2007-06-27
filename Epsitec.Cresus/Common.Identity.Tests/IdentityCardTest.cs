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
		public void CheckSerialization()
		{
			IdentityCard card = new IdentityCard ();

			card.DeveloperId = 123;
			card.RawImage = new byte[] { 1, 2, 3 };
			card.UserName = "Jean Dupont";

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (card);

			System.Console.Out.WriteLine (xml);

			IdentityCard restoredCard = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityCard;

			Assert.AreEqual (card.DeveloperId, restoredCard.DeveloperId);
			Assert.AreEqual (card.RawImage, restoredCard.RawImage);
			Assert.AreEqual (card.UserName, restoredCard.UserName);
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

			repository.Identities.Add (cardChristian);	//	Alleyn
			repository.Identities.Add (cardPierre);		//	Arnaud
			repository.Identities.Add (cardDavid);		//	Besuchet
			repository.Identities.Add (cardYves);		//	Raboud
			repository.Identities.Add (cardDaniel);		//	Roux

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (repository);

			System.IO.File.WriteAllText (System.IO.Path.Combine (path, "identities.xml"), xml);
		}
	}
}
