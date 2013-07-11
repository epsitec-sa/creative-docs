using System;


namespace Epsitec.Aider.Enumerations
{


	public static class EnumUtils
	{



		public static PersonSex GuessSex(string title)
		{
			return EnumUtils.GuessSex (TextParser.ParsePersonMrMrs (title));
		}


		public static PersonSex GuessSex(PersonMrMrs title)
		{
			switch (title)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
					return PersonSex.Female;

				case PersonMrMrs.Monsieur:
					return PersonSex.Male;

				case PersonMrMrs.None:
					return PersonSex.Unknown;

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
