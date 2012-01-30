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


		public static string AsText(this PersonMrMrs honorific)
		{
			switch (honorific)
			{
				case PersonMrMrs.Madame:
					return "Madame";

				case PersonMrMrs.Mademoiselle:
					return "Mademoiselle";

				case PersonMrMrs.Monsieur:
					return "Monsieur";

				case PersonMrMrs.None:
					return null;

				default:
					throw new NotImplementedException ();
			}
		}


		public static string AsText(this PersonConfession confession)
		{
			switch (confession)
			{
				case PersonConfession.Anglican:
					return "Anglican";

				case PersonConfession.Buddhist:
					return "Bouddhiste";
				
				case PersonConfession.Catholic:
					return "Catolhique";
				
				case PersonConfession.Darbyst:
					return "Darbyste";
				
				case PersonConfession.Evangelic:
					return "Evangélique";
				
				case PersonConfession.Israelite:
					return "Israélite";
				
				case PersonConfession.JehovahsWitness:
					return "Témoin de Jéhovah";
				
				case PersonConfession.Muslim:
					return "Musulman";
				
				case PersonConfession.NewApostolic:
					return "Néo-apostolique";
				
				case PersonConfession.Orthodox:
					return "Orthodoxe";
				
				case PersonConfession.Protestant:
					return "Protestant";
				
				case PersonConfession.SalvationArmy:
					return "Armée du salut";

				case PersonConfession.None:
				case PersonConfession.Unknown:
					return null;

				default:
					throw new NotImplementedException ();
			}
		}


		public static string AsText(this AddressType type)
		{
			switch (type)
			{
				case AddressType.Default:
					return null;

				case AddressType.Private:
					return "Privé";

				case AddressType.Professional:
					return "Professionnel";

				case AddressType.Secondary:
					return "Secondaire";

				default:
					return null;
			}
		}


		public static string AsText(this HouseholdMrMrs honorific)
		{
			switch (honorific)
			{
				case HouseholdMrMrs.Famille:
					return "Famille";

				case HouseholdMrMrs.MadameEtMonsieur:
					return "Madame et Monsieur";

				case HouseholdMrMrs.MonsieurEtMadame:
					return "Monsieur et Madame";

				case HouseholdMrMrs.None:
				case HouseholdMrMrs.Auto:
					return null;

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
