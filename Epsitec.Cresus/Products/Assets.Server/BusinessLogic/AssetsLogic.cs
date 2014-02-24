//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AssetsLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'un objet, du genre:
			//	"Toyota Yaris Verso"
			var obj = accessor.GetObject (BaseType.Assets, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				//	On prend les champs de type texte ayant un SummaryOrder.
				var list = new List<string> ();

				foreach (var field in accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.String && x.SummaryOrder.HasValue)
					.OrderBy (x => x.SummaryOrder)
					.Select (x => x.Field))
				{
					var text = ObjectProperties.GetObjectPropertyString (obj, null, field);

					if (!string.IsNullOrEmpty (text))
					{
						list.Add (text);
					}
				}

				return string.Join (" ", list).Trim ();
			}
		}
	}
}
