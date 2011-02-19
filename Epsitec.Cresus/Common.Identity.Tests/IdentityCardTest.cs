//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
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
		public void CheckUserInterfaceDialog()
		{
			IList<IdentityCard> cards = IdentityRepository.Default.IdentityCards;
			UI.IdentityCardSelectorDialog dialog = new UI.IdentityCardSelectorDialog (cards);

			using (Tool.InjectKey (System.Windows.Forms.Keys.Return))
			{
				dialog.OpenDialog ();
			}
		}

		[Test]
		public void CheckUserInterfaceWidget()
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

			IdentityCard cardCA = new IdentityCard ("Christian Alleyn",   5, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "CA.png")));
			IdentityCard cardPA = new IdentityCard ("Pierre Arnaud",      1, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "PA.png")));
			IdentityCard cardDB = new IdentityCard ("David Besuchet",     3, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "DB.png")));
			IdentityCard cardMB = new IdentityCard ("Marc Bettex",        8, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "MB.png")));
			IdentityCard cardDD = new IdentityCard ("Denis Dumoulin",     6, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "DD.png")));
			IdentityCard cardYR = new IdentityCard ("Yves Raboud",        4, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "YR.png")));
			IdentityCard cardDR = new IdentityCard ("Daniel Roux",        2, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "DR.png")));
			IdentityCard cardJS = new IdentityCard ("Jonas Schmid",       9, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "JS.png")));
			IdentityCard cardMS = new IdentityCard ("Mathieu Schroeter", 10, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "MS.png")));
			IdentityCard cardMW = new IdentityCard ("Michael Walz",       7, System.IO.File.ReadAllBytes (System.IO.Path.Combine (path, "MW.png")));

			repository.IdentityCards.Add (cardCA);
			repository.IdentityCards.Add (cardPA);
			repository.IdentityCards.Add (cardDB);
			repository.IdentityCards.Add (cardMB);
			repository.IdentityCards.Add (cardDD);
			repository.IdentityCards.Add (cardYR);
			repository.IdentityCards.Add (cardDR);
			repository.IdentityCards.Add (cardJS);
			repository.IdentityCards.Add (cardMS);
			repository.IdentityCards.Add (cardMW);

			string xml = Epsitec.Common.Types.Serialization.SimpleSerialization.SerializeToString (repository);

			System.IO.File.WriteAllText (System.IO.Path.Combine (path, IdentityRepository.DefaultIdentitiesFileName), xml);

			IdentityRepository restoredRepository = Epsitec.Common.Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as IdentityRepository;
		}
	}
}
