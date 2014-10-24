//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AssetsLogic
	{
		public static DataObject CreateAsset(DataAccessor accessor, System.DateTime date, IEnumerable<AbstractDataProperty> requiredProperties, decimal? value, Guid cat)
		{
			//	Crée un nouvel objet d'immobilisation.
			var guid = accessor.CreateObject (BaseType.Assets, date, Guid.Empty, requiredProperties.ToArray ());
			var asset = accessor.GetObject (BaseType.Assets, guid);
			System.Diagnostics.Debug.Assert (asset != null);

			//	Importe la catégorie d'immobilisation dans l'événement d'entrée, si nécessaire.
			if (!cat.IsEmpty)
			{
				CategoriesLogic.ImportCategoryToAsset (accessor, asset, null, cat);
			}

			//	Crée la valeur comptable d'entrée, si nécessaire.
			if (value.HasValue)
			{
				var e = asset.GetEvent (0);  // événement d'entrée
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				var aa = p.Value;

				aa = AmortizedAmount.SetInitialAmount (aa, value.Value);
				aa = Entries.CreateEntry (accessor, aa);  // génère ou met à jour les écritures
				Amortizations.SetAmortizedAmount (e, aa);
			}

			return asset;
		}


		public static IEnumerable<UserField> GetUserFields(DataAccessor accessor)
		{
			//	Retourne les champs d'un objet d'immobilisation.
			int index = 0;

			foreach (var userField in accessor.GlobalSettings.GetUserFields (BaseType.AssetsUserFields))
			{
				if (index == 2)
				{
					//	Juste après les deux premières colonnes (en général nom et numéro), on injecte
					//	la valeur comptable.
					//	Le Guid créé à la volée n'est pas utilisé !
					yield return new UserField (DataDescriptions.GetObjectFieldDescription (ObjectField.MainValue), ObjectField.MainValue, FieldType.AmortizedAmount, false, 120, null, null, null, 0);
				}

				yield return userField;
				index++;
			}
		}


		public static string GetSummary(DataAccessor accessor, Guid guid, Timestamp? timestamp = null)
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

				foreach (var field in accessor.GlobalSettings.GetUserFields (BaseType.AssetsUserFields)
					.Where (x => x.Type == FieldType.String && x.SummaryOrder.HasValue)
					.OrderBy (x => x.SummaryOrder)
					.Select (x => x.Field))
				{
					var text = ObjectProperties.GetObjectPropertyString (obj, timestamp, field);

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
