//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class EnumDictionaries
	{
		public static string GetPeriodicityName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetPeriodicityName ((Periodicity) type.Value);
			}

			return null;
		}

		public static string GetAmortizationTypeName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetAmortizationTypeName ((AmortizationType) type.Value);
			}

			return null;
		}

		public static string GetProrataTypeName(int? type)
		{
			if (type.HasValue)
			{
				return EnumDictionaries.GetProrataTypeName ((ProrataType) type.Value);
			}

			return null;
		}


		public static string GetPeriodicityName(Periodicity type)
		{
			string s;
			if (EnumDictionaries.DictPeriodicities.TryGetValue ((int) type, out s))
			{
				return s;
			}

			return null;
		}

		public static string GetAmortizationTypeName(AmortizationType type)
		{
			string s;
			if (EnumDictionaries.DictAmortizationTypes.TryGetValue ((int) type, out s))
			{
				return s;
			}

			return null;
		}

		public static string GetProrataTypeName(ProrataType type)
		{
			string s;
			if (EnumDictionaries.DictProrataTypes.TryGetValue ((int) type, out s))
			{
				return s;
			}

			return null;
		}

		public static string GetFieldTypeName(FieldType type)
		{
			string s;
			if (EnumDictionaries.GetDictFieldTypes ().TryGetValue ((int) type, out s))
			{
				return s;
			}

			return null;
		}


		//	Ici, il est préférable de ne pas avoir de mécanisme automatique pour
		//	générer les dictionnaires à partir des enumérations C#. En effet, les
		//	énumérations peuvent évoluer au cours du temps, de nouvelles valeurs
		//	peuvent être introduites. Le dictionnaire est dans un ordre logique
		//	pour l'utilisateur, qui n'est pas forcément le même que celui de
		//	l'énumération C#.

		public static Dictionary<int, string> DictPeriodicities
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				dict.Add ((int) Periodicity.Annual,      "Annuel");
				dict.Add ((int) Periodicity.Semestrial,  "Semestriel");
				dict.Add ((int) Periodicity.Trimestrial, "Trimestriel");
				dict.Add ((int) Periodicity.Mensual,     "Mensuel");

				return dict;
			}
		}

		public static Dictionary<int, string> DictAmortizationTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				dict.Add ((int) AmortizationType.Linear,     "Linéaire");
				dict.Add ((int) AmortizationType.Degressive, "Dégressif");

				return dict;
			}
		}

		public static Dictionary<int, string> DictProrataTypes
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				dict.Add ((int) ProrataType.None,       "Aucun");
				dict.Add ((int) ProrataType.Prorata365, "Sur 365 jours");
				dict.Add ((int) ProrataType.Prorata360, "Sur 360 jours");
				dict.Add ((int) ProrataType.Prorata12,  "Sur 12 mois");

				return dict;
			}
		}

		public static Dictionary<int, string> GetDictFieldTypes(bool hasComplexTypes = true)
		{
			var dict = new Dictionary<int, string> ();

			dict.Add ((int) FieldType.String, "Texte");

			if (hasComplexTypes)
			{
				dict.Add ((int) FieldType.ComputedAmount, "Montant");
			}

			dict.Add ((int) FieldType.Decimal, "Nombre réel");
			dict.Add ((int) FieldType.Int,     "Nombre entier");
			dict.Add ((int) FieldType.Date,    "Date");

			if (hasComplexTypes)
			{
				dict.Add ((int) FieldType.GuidPerson, "Personne");
			}

			return dict;
		}
	}
}
