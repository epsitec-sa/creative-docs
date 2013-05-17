namespace Epsitec.Aider.Enumerations
{


	internal static class TextParser
	{


		public static PersonMrMrs ParsePersonMrMrs(string personMrMrs)
		{
			switch ((personMrMrs ?? "").ToLowerInvariant())
			{
				case "monsieur":
					return PersonMrMrs.Monsieur;

				case "madame":
					return PersonMrMrs.Madame;

				case "mademoiselle":
					return PersonMrMrs.Mademoiselle;

				default:
					return PersonMrMrs.None;
			}
		}


	}


}
