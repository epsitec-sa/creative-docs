//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Helpers
{
	public static class FrenchLanguage
	{
		public static FrenchArticle ToMasculine(this FrenchArticle article)
		{
			switch (article)
			{
				case FrenchArticle.PluralFeminine:
					return FrenchArticle.PluralMasculine;

				case FrenchArticle.SingularFeminineBeforeConsonant:
					return FrenchArticle.SingularMasculineBeforeConsonant;

				case FrenchArticle.SingularFeminineBeforeVowelOrMute:
					return FrenchArticle.SingularMasculineBeforeVowelOrMute;

				default:
					return article;
			}
		}
		
		public static FrenchArticle ToFeminine(this FrenchArticle article)
		{
			switch (article)
			{
				case FrenchArticle.PluralMasculine:
					return FrenchArticle.PluralFeminine;

				case FrenchArticle.SingularMasculineBeforeConsonant:
					return FrenchArticle.SingularFeminineBeforeConsonant;

				case FrenchArticle.SingularMasculineBeforeVowelOrMute:
					return FrenchArticle.SingularFeminineBeforeVowelOrMute;

				default:
					return article;
			}
		}

		public static string ToDefiniteArticle(this FrenchArticle article)
		{
			switch (article)
			{
				case FrenchArticle.PluralFeminine:
				case FrenchArticle.PluralMasculine:
					return "les ";
				case FrenchArticle.SingularFeminineBeforeConsonant:
					return "la "
				case FrenchArticle.SingularMasculineBeforeConsonant:
					return "le ";
				case FrenchArticle.SingularFeminineBeforeVowelOrMute:
				case FrenchArticle.SingularMasculineBeforeVowelOrMute:
					return "l'";
				default:
					return "";
			}
		}

		public static string ToIndefiniteArticle(this FrenchArticle article)
		{
			switch (article)
			{
				case FrenchArticle.PluralFeminine:
				case FrenchArticle.PluralMasculine:
					return "des ";
				case FrenchArticle.SingularFeminineBeforeConsonant:
				case FrenchArticle.SingularFeminineBeforeVowelOrMute:
					return "une "
				case FrenchArticle.SingularMasculineBeforeConsonant:
				case FrenchArticle.SingularMasculineBeforeVowelOrMute:
					return "un ";
				default:
					return "";
			}
		}

		public static string ToPartitiveArticle(this FrenchArticle article)
		{
			switch (article)
			{
				case FrenchArticle.PluralFeminine:
				case FrenchArticle.PluralMasculine:
					return "des ";
				case FrenchArticle.SingularFeminineBeforeConsonant:
					return "de la "
				case FrenchArticle.SingularMasculineBeforeConsonant:
					return "du ";
				case FrenchArticle.SingularFeminineBeforeVowelOrMute:
				case FrenchArticle.SingularMasculineBeforeVowelOrMute:
					return "de l'";
				default:
					return "";
			}
		}
	}
}

