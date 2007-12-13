using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

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
			prix1.Monnaie.D�signation = "CHF";

			DialogData data = new DialogData (prix1, DialogDataMode.Isolated);

			PrixEntity prix2 = data.Data as PrixEntity;

			Assert.AreEqual (10.0M, prix2.Ht);
			Assert.AreEqual ("CHF", prix2.Monnaie.D�signation);

			prix2.Ht = 15.0M;
			prix2.Monnaie.D�signation = "EUR";
			
			Assert.AreEqual (10.0M, prix1.Ht);
			Assert.AreEqual ("CHF", prix1.Monnaie.D�signation);
			Assert.AreEqual (15.0M, prix2.Ht);
			Assert.AreEqual ("EUR", prix2.Monnaie.D�signation);

			data.ApplyChanges (change => System.Console.Out.WriteLine ("Change {0} from {1} to {2}", change.Path, change.OldValue, change.NewValue));
		}
		
		[Test]
		public void Check02DialogModeRealTime()
		{
			EntityContext context = EntityContext.Current;
			PrixEntity prix1 = context.CreateEmptyEntity<PrixEntity> ();
			prix1.Monnaie = context.CreateEmptyEntity<MonnaieEntity> ();
			prix1.Ht = 10.0M;
			prix1.Monnaie.D�signation = "CHF";

			DialogData data = new DialogData (prix1, DialogDataMode.RealTime);

			PrixEntity prix2 = data.Data as PrixEntity;

			Assert.AreEqual (10.0M, prix2.Ht);
			Assert.AreEqual ("CHF", prix2.Monnaie.D�signation);

			prix2.Ht = 15.0M;
			prix2.Monnaie.D�signation = "EUR";

			Assert.AreEqual (15.0M, prix1.Ht);
			Assert.AreEqual ("EUR", prix1.Monnaie.D�signation);
			Assert.AreEqual (15.0M, prix2.Ht);
			Assert.AreEqual ("EUR", prix2.Monnaie.D�signation);

			data.ApplyChanges (change => System.Console.Out.WriteLine ("Change {0} from {1} to {2}", change.Path, change.OldValue, change.NewValue));
		}
	}
}
