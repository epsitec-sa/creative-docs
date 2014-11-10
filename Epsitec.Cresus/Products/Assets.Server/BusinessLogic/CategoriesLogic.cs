//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class CategoriesLogic
	{
		#region Import category to asset
		public static void ImportCategoryToAsset(DataAccessor accessor, DataObject asset, DataEvent e, Guid catGuid)
		{
			//	Importe par copie une catégorie d'immobilisation dans un objet d'immobilisation.
			//	Si asset = null, on importe dans l'objet en édition.

			if (asset != null && e == null)
			{
				e = asset.GetEvent (0);  // prend l'événement d'entrée
			}

			var catObj = accessor.GetObject (BaseType.Categories, catGuid);

			//	Copie les champs nécessaires.
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.Name,                           ObjectField.CategoryName);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.AmortizationRate,               ObjectField.AmortizationRate);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.AmortizationType,               ObjectField.AmortizationType);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.Periodicity,                    ObjectField.Periodicity);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.Prorata,                        ObjectField.Prorata);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.Round,                          ObjectField.Round);
			CategoriesLogic.ImportField (accessor, asset, e, catObj, ObjectField.ResidualValue,                  ObjectField.ResidualValue);

			foreach (var field in DataAccessor.AccountFields)
			{
				CategoriesLogic.ImportField (accessor, asset, e, catObj, field, field);
			}
		}

		private static void ImportField(DataAccessor accessor, DataObject asset, DataEvent e, DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var typeSrc = accessor.GetFieldType (fieldSrc);
			var typeDst = accessor.GetFieldType (fieldDst);
			System.Diagnostics.Debug.Assert (typeSrc == typeDst);

			switch (typeSrc)
			{
				case FieldType.String:
				case FieldType.Account:
					CategoriesLogic.ImportFieldString (accessor, asset, e, catObj, fieldSrc, fieldDst);
					break;

				case FieldType.Decimal:
					CategoriesLogic.ImportFieldDecimal (accessor, asset, e, catObj, fieldSrc, fieldDst);
					break;

				case FieldType.Int:
					CategoriesLogic.ImportFieldInt (accessor, asset, e, catObj, fieldSrc, fieldDst);
					break;

				default:
					System.Diagnostics.Debug.Fail ("Not supported");
					break;
			}
		}

		private static void ImportFieldString(DataAccessor accessor, DataObject asset, DataEvent e, DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var s = ObjectProperties.GetObjectPropertyString (catObj, null, fieldSrc);
			if (!string.IsNullOrEmpty (s))
			{
				if (asset == null)
				{
					accessor.EditionAccessor.SetField (fieldDst, s);
				}
				else
				{
					var newProperty = new DataStringProperty (fieldDst, s);
					e.AddProperty (newProperty);
				}
			}
		}

		private static void ImportFieldDecimal(DataAccessor accessor, DataObject asset, DataEvent e, DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectProperties.GetObjectPropertyDecimal (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				if (asset == null)
				{
					accessor.EditionAccessor.SetField (fieldDst, d);
				}
				else
				{
					var newProperty = new DataDecimalProperty (fieldDst, d.Value);
					e.AddProperty (newProperty);
				}
			}
		}

		private static void ImportFieldInt(DataAccessor accessor, DataObject asset, DataEvent e, DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectProperties.GetObjectPropertyInt (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				if (asset == null)
				{
					accessor.EditionAccessor.SetField (fieldDst, d);
				}
				else
				{
					var newProperty = new DataIntProperty (fieldDst, d.Value);
					e.AddProperty (newProperty);
				}
			}
		}
#endregion


		public static bool HasAccounts(DataAccessor accessor)
		{
			//	Indique s'il existe une catégorie d'immobilisation qui fait référence à un compte.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				if (CategoriesLogic.HasAccounts (cat))
				{
					return true;
				}
			}

			return false;
		}

		private static bool HasAccounts(DataObject cat)
		{
			return DataAccessor.AccountFields.Where (x => CategoriesLogic.HasAccounts (cat, x)).Any ();
		}

		private static bool HasAccounts(DataObject cat, ObjectField field)
		{
			var account = ObjectProperties.GetObjectPropertyString (cat, null, field);
			return !string.IsNullOrEmpty (account);
		}


		public static void ClearAccounts(DataAccessor accessor)
		{
			//	Efface toutes les références aux comptes dans toutes les catégories d'immobilisation.
			foreach (var cat in accessor.Mandat.GetData (BaseType.Categories))
			{
				CategoriesLogic.ClearAccounts (cat);
			}
		}

		private static void ClearAccounts(DataObject cat)
		{
			var e = cat.GetEvent (0);

			foreach (var field in DataAccessor.AccountFields)
			{
				e.RemoveProperty (field);
			}
		}


		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une catégorie, du genre:
			//	"Véhicules"
			var obj = accessor.GetObject (BaseType.Categories, guid);
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}


		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'une catégorie, du genre:
			//	"Véhicules 12.5% Linéaire Annuel"
			var obj = accessor.GetObject (BaseType.Categories, guid);
			if (obj == null)
			{
				return null;
			}

			var name   = ObjectProperties.GetObjectPropertyString  (obj, null, ObjectField.Name);
			var taux   = ObjectProperties.GetObjectPropertyDecimal (obj, null, ObjectField.AmortizationRate);
			var type   = ObjectProperties.GetObjectPropertyInt     (obj, null, ObjectField.AmortizationType);
			var period = ObjectProperties.GetObjectPropertyInt     (obj, null, ObjectField.Periodicity);

			var stringTaux   = TypeConverters.RateToString (taux);
			var stringType   = EnumDictionaries.GetAmortizationTypeName (type);
			var stringPeriod = EnumDictionaries.GetPeriodicityName (period);

			return string.Format ("{0} {1} {2} {3}", name, stringTaux, stringType, stringPeriod);
		}
	}
}
