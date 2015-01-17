//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class EntriesLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé d'une écriture.
			var entry = accessor.GetObject (BaseType.Entries, guid);
			if (entry == null)
			{
				return null;
			}
			else
			{
				var n = ObjectProperties.GetObjectPropertyString (entry, null, ObjectField.EntryStamp);
				var t = ObjectProperties.GetObjectPropertyString (entry, null, ObjectField.EntryTitle);

				return string.Join (" ", n, t);
			}
		}
	}
}
