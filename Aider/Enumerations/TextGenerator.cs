//	Copyright © 2011-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	public static class TextGenerator
	{
		public static string GetText(this PersonMrMrs? value, bool abbreviated)
		{
			return TextGenerator.GetText (value ?? PersonMrMrs.None, abbreviated);
		}

		public static string GetText(this PersonMrMrs value, bool abbreviated = false)
		{
			return abbreviated
				? TextGenerator.GetShortText (value)
				: TextGenerator.GetLongText (value);
		}

		public static string GetLongText(this PersonMrMrs? value)
		{
			return TextGenerator.GetLongText (value ?? PersonMrMrs.None);
		}

		public static string GetLongText(this PersonMrMrs value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
				case PersonMrMrs.Monsieur:
					return EnumKeyValues.GetEnumKeyValue (value).Values.First ().ToSimpleText ();

				case PersonMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}

		public static string GetLongText(this PersonMrMrsTitle? value)
		{
			return TextGenerator.GetLongText (value ?? PersonMrMrsTitle.Auto);
		}

		public static string GetLongText(this PersonMrMrsTitle value)
		{
			switch (value)
			{
				default:
					return EnumKeyValues.GetEnumKeyValue (value).Values.First ().ToSimpleText ();

				case PersonMrMrsTitle.Auto:
					return "";
			}
		}

		public static string GetShortText(this PersonMrMrs? value)
		{
			return TextGenerator.GetShortText (value ?? PersonMrMrs.None);
		}

		public static string GetShortText(this PersonMrMrs value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
				case PersonMrMrs.Monsieur:
					return EnumKeyValues.GetEnumKeyValue (value).Values.Last ().ToSimpleText ();

				case PersonMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}
		
		public static string GetShortText(this PersonMrMrsTitle? value)
		{
			return TextGenerator.GetShortText (value ?? PersonMrMrsTitle.Auto);
		}

		public static string GetShortText(this PersonMrMrsTitle value)
		{
			switch (value)
			{
				default:
					return EnumKeyValues.GetEnumKeyValue (value).Values.Last ().ToSimpleText ();

				case PersonMrMrsTitle.Auto:
					return "";
			}
		}

		public static string GetText(this HouseholdMrMrs value, bool abbreviated = false)
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
				case HouseholdMrMrs.Messieurs:			return "Messieurs";
				case HouseholdMrMrs.Mesdames:			return "Mesdames";

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
				case HouseholdMrMrs.Messieurs:			return "MM.";
				case HouseholdMrMrs.Mesdames:			return "Mmes";

				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.None:
					return "";

				default:
					throw new System.NotImplementedException ();
			}
		}
	}
}
