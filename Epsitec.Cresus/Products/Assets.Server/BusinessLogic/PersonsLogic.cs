//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class PersonsLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le résumé d'une personne, du genre:
			//	"Jean Dupond Epsitec SA"
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				//	On prend les champs de type texte ayant un SummaryOrder.
				var list = new List<string> ();

				foreach (var field in accessor.UserFieldsCache.GetUserFields (BaseType.PersonsUserFields)
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


		public static string GetFullDescription(DataAccessor accessor, Guid guid)
		{
			//	Retourne la description multi-lignes d'une personne, du genre:
			//	Dupond
			//	Jean
			//	Monsieur
			//	Epsitec SA
			//	Rue de Neuchâtel 32
			//	1400
			//	Yverdon-les-Bains
			//	Suisse
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				var lines = new List<string> ();

				foreach (var field in accessor.UserFieldsCache.GetUserFields (BaseType.PersonsUserFields)
					.Select (x => x.Field))
				{
					var text = ObjectProperties.GetObjectPropertyString (obj, null, field);
					PersonsLogic.PutLine (lines, text);
				}

				return string.Join ("<br/>", lines);
			}
		}

		private static void PutLine(List<string> lines, params string[] words)
		{
			var line = string.Join (" ", words
				.Where (x => !string.IsNullOrEmpty (x))
				.Select (x => x.Replace ("<br/>", ", "))).Trim ();

			if (!string.IsNullOrEmpty (line))
			{
				lines.Add (line);
			}
		}
	}
}
