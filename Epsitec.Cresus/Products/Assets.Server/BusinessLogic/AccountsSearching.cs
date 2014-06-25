//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AccountsSearching
	{
		public static DataObject ApproximativeTitleSearch(GuidList<DataObject> accounts, DataObject searching)
		{
			//	Retourne le compte dont le titre s'approche le plus possible.
			var searchingTitle = ObjectProperties.GetObjectPropertyString (searching, null, ObjectField.Name);

			int bestRanking = 0;  // pas trouvé
			DataObject bestAccount = null;

			foreach (var account in accounts)
			{
				var accountTitle = ObjectProperties.GetObjectPropertyString (account, null, ObjectField.Name);
				var ranking = ApproximativeSearching.GetRanking (accountTitle, searchingTitle);

				if (bestRanking < ranking)  // a-t-on trouvé mieux ?
				{
					bestRanking = ranking;
					bestAccount = account;
				}
			}

			return bestAccount;
		}
	}
}
