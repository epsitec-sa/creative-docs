//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyAccounts
	{
		public static void AddAccounts(DataMandat mandat)
		{
			var accounts = mandat.GetData (BaseType.Accounts);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o0 = new DataObject ();
			accounts.Add (o0);
			{
				var e = new DataEvent (start, EventType.Input);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyAccounts.AccountNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Comptes"));
			}

			///////////////

			var oActif = new DataObject ();
			accounts.Add (oActif);
			{
				var e = new DataEvent (start, EventType.Input);
				oActif.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyAccounts.AccountNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "10"));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Actif"));
			}

			var o1 = new DataObject ();
			accounts.Add (o1);
			{
				var e = new DataEvent (start, EventType.Input);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyAccounts.AccountNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oActif.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "1000"));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Caisse"));
			}

		}

		private static int AccountNumber = 1;
	}

}