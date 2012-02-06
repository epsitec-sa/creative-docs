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
	}
}
