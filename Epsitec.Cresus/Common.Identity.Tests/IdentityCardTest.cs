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

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (card);

			System.Console.Out.WriteLine (xml);

			IdentityCard restoredCard = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityCard;

			Assert.AreEqual (card.DeveloperId, restoredCard.DeveloperId);
			Assert.AreEqual (card.RawImage, restoredCard.RawImage);
		}
	}
}
