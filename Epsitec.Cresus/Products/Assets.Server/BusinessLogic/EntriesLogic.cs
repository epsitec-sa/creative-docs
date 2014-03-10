//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class EntriesLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé d'une écriture.
			var entry = EntriesLogic.GetEntry (accessor, guid);
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

		private static DataObject GetEntry(DataAccessor accessor, Guid guid)
		{
			//	TODO: ci-dessous
			//	Bof, quelle horreur de faire ainsi !
			//	Faut-il remettre toutes les écritures dans BaseType.Entries ?
			var assets = accessor.Mandat.GetData (BaseType.Assets);

			for (int i=0; i<assets.Count; i++)
			{
				var asset = assets[i];

				foreach (var e in asset.Events)
				{
					var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

					if (p != null)
					{
						var aa = p.Value;

						foreach (var entry in aa.Entries)
						{
							if (entry.Guid == guid)
							{
								return entry;
							}
						}
					}
				}
			}

			return null;
		}
	}
}
