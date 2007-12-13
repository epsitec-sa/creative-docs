using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

using Demo.Demo5juin.Entities;

namespace Epsitec.Common.Dialogs
{
	[TestFixture]
	public class DialogDataTest
	{
		[Test]
		public void Check01DialogModeIsolated()
		{
			EntityContext context = EntityContext.Current;
			PrixEntity prix1 = context.CreateEmptyEntity<PrixEntity> ();
			prix1.Monnaie = context.CreateEmptyEntity<MonnaieEntity> ();
			prix1.Ht = 10.0M;
			prix1.Monnaie.Désignation = "CHF";

			DialogData data = new DialogData (prix1, DialogDataMode.Isolated);

			PrixEntity prix2 = data.Data as PrixEntity;

			Assert.AreEqual (10.0M, prix2.Ht);
			Assert.AreEqual ("CHF", prix2.Monnaie.Désignation);

			prix2.Ht = 15.0M;
			prix2.Monnaie.Désignation = "EUR";
			
			Assert.AreEqual (10.0M, prix1.Ht);
			Assert.AreEqual ("CHF", prix1.Monnaie.Désignation);
			Assert.AreEqual (15.0M, prix2.Ht);
			Assert.AreEqual ("EUR", prix2.Monnaie.Désignation);

			List<string> results = new List<string> ();

			data.ApplyChanges (change => results.Add (string.Format ("Change {0} from {1} to {2}", change.Path, change.OldValue, change.NewValue)));

			Collection.CompareEqual (results,
				new string[]
				{
					"Change [630G].[630A] from CHF to EUR",
					"Change [630H] from 10.0 to 15.0"
				});
		}
		
		[Test]
		public void Check02DialogModeRealTime()
		{
			EntityContext context = EntityContext.Current;
			PrixEntity prix1 = context.CreateEmptyEntity<PrixEntity> ();
			prix1.Monnaie = context.CreateEmptyEntity<MonnaieEntity> ();
			prix1.Ht = 10.0M;
			prix1.Monnaie.Désignation = "CHF";

			DialogData data = new DialogData (prix1, DialogDataMode.RealTime);

			PrixEntity prix2 = data.Data as PrixEntity;

			Assert.AreEqual (10.0M, prix2.Ht);
			Assert.AreEqual ("CHF", prix2.Monnaie.Désignation);

			prix2.Ht = 15.0M;
			prix2.Monnaie.Désignation = "EUR";

			Assert.AreEqual (15.0M, prix1.Ht);
			Assert.AreEqual ("EUR", prix1.Monnaie.Désignation);
			Assert.AreEqual (15.0M, prix2.Ht);
			Assert.AreEqual ("EUR", prix2.Monnaie.Désignation);

			List<string> results = new List<string> ();

			data.ApplyChanges (change => results.Add (string.Format ("Change {0} from {1} to {2}", change.Path, change.OldValue, change.NewValue)));

			Collection.CompareEqual (results,
				new string[]
				{
					"Change [630G].[630A] from CHF to EUR",
					"Change [630H] from 10.0 to 15.0"
				});
		}
	}
}
