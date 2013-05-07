//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	public static class TextGenerator
	{
		public static string GetText(this PersonMrMrs value, bool abbreviated)
		{
			return abbreviated
				? TextGenerator.GetShortText (value)
				: TextGenerator.GetLongText (value);
		}

		public static string GetText(this PersonMrMrs? value, bool abbreviated)
		{
			return abbreviated
				? TextGenerator.GetShortText (value)
				: TextGenerator.GetLongText (value);
		}

		public static string GetLongText(this PersonMrMrs? value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
				case PersonMrMrs.Monsieur:
					return EnumKeyValues.GetEnumKeyValue (value.Value).Values.First ().ToSimpleText ();

				case null:
				case PersonMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}

		public static string GetShortText(this PersonMrMrs? value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
				case PersonMrMrs.Monsieur:
					return EnumKeyValues.GetEnumKeyValue (value.Value).Values.Last ().ToSimpleText ();

				case null:
				case PersonMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}

		public static string GetText(this HouseholdMrMrs value, bool abbreviated)
		{
			return abbreviated
				? value.GetShortText ()
				: value.GetLongText ();
		}

		public static string GetLongText(this HouseholdMrMrs value)
		{
			switch (value)
			{
				case HouseholdMrMrs.Famille:			return "Famille";
				case HouseholdMrMrs.MadameEtMonsieur:	return "Madame et Monsieur";
				case HouseholdMrMrs.MonsieurEtMadame:	return "Monsieur et Madame";

				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}

		public static string GetShortText(this HouseholdMrMrs value)
		{
			switch (value)
			{
				case HouseholdMrMrs.Famille:			return "Fam.";
				case HouseholdMrMrs.MadameEtMonsieur:	return "Mme et M.";
				case HouseholdMrMrs.MonsieurEtMadame:	return "M. et Mme";

				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}
	}
}
