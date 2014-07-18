//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class WarningsLogic
	{
		public static void GetWarnings(List<Warning> warnings, DataAccessor accessor)
		{
			//	Retourne la liste de tous les warnings actuels.
			warnings.Clear ();

			foreach (var asset in accessor.Mandat.GetData (BaseType.Assets))
			{
				foreach (var e in asset.Events)
				{
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account1);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account2);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account3);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account4);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account5);
					WarningsLogic.CheckAccount (warnings, accessor, asset, e, ObjectField.Account6);
				}
			}
		}

		private static void CheckAccount(List<Warning> warnings, DataAccessor accessor, DataObject asset, DataEvent e, ObjectField accountField)
		{
			var p = e.GetProperty (accountField) as DataStringProperty;
			if (p != null && !string.IsNullOrEmpty (p.Value))
			{
				var baseType = accessor.Mandat.GetAccountsBase (e.Timestamp.Date);
				var account = AccountsLogic.GetAccount(accessor, baseType, p.Value);

				if (account == null)
				{
					var desc = string.Format ("Le compte {0} n'est pas défini", p.Value);
					var warning = new Warning (BaseType.Assets, "View.Assets", asset.Guid, e.Guid, accountField, desc);
					warnings.Add (warning);
				}
			}
		}
	}
}
