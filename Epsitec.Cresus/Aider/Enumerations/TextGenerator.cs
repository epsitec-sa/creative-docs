using System;

namespace Epsitec.Aider.Enumerations
{
	public static class TextGenerator
	{
		public static string AsShortText(this PersonSex sex)
		{
			switch (sex)
			{
				case PersonSex.Female:
					return "F";

				case PersonSex.Male:
					return "H";

				case PersonSex.Unknown:
					return null;

				default:
					throw new NotImplementedException ();
			}
		}


		public static string AsShortText(this Language language)
		{
			switch (language)
			{
				case Language.French:
					return "Fr";

				case Language.German:
					return "De";

				default:
					throw new NotImplementedException ();
			}
		}

		public static string GetText(this PersonMrMrs value, bool abbreviated)
		{
			return abbreviated
				? value.GetShortText ()
				: value.GetLongText ();
		}

		public static string GetLongText(this PersonMrMrs value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
					return "Madame";

				case PersonMrMrs.Mademoiselle:
					return "Mademoiselle";

				case PersonMrMrs.Monsieur:
					return "Monsieur";

				case PersonMrMrs.None:
					return "";

				default:
					throw new NotImplementedException ();
			}
		}

		public static string GetShortText(this PersonMrMrs value)
		{
			switch (value)
			{
				case PersonMrMrs.Madame:
					return "Mme";

				case PersonMrMrs.Mademoiselle:
					return "Mlle.";

				case PersonMrMrs.Monsieur:
					return "M.";

				case PersonMrMrs.None:
					return "";

				default:
					throw new NotImplementedException ();
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
				case HouseholdMrMrs.Famille:
					return "Famille";

				case HouseholdMrMrs.MadameEtMonsieur:
					return "Madame et Monsieur";

				case HouseholdMrMrs.MonsieurEtMadame:
					return "Monsieur et Madame";

				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.None:
					return "";

				default:
					throw new NotImplementedException ();
			}
		}

		public static string GetShortText(this HouseholdMrMrs value)
		{
			switch (value)
			{
				case HouseholdMrMrs.Famille:
					return "Fam.";

				case HouseholdMrMrs.MadameEtMonsieur:
					return "Mme. et M.";

				case HouseholdMrMrs.MonsieurEtMadame:
					return "M. et Mme.";

				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.None:
					return "";

				default:
					throw new NotImplementedException ();
			}
		}
	}
}
