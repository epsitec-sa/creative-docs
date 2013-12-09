//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetObjectPageDescription(EditionObjectPageType type)
		{
			switch (type)
			{
				case EditionObjectPageType.OneShot:
					return "Evénement";

				case EditionObjectPageType.Summary:
					return "Résumé";

				case EditionObjectPageType.Object:
					return "Général";

				case EditionObjectPageType.Values:
					return "Valeurs";

				case EditionObjectPageType.Amortissements:
					return "Amortissements";

				case EditionObjectPageType.Groups:
					return "Regroupements";

				case EditionObjectPageType.Category:
					return "Définitions de la catégorie";

				case EditionObjectPageType.Group:
					return "Définitions du groupe";

				case EditionObjectPageType.Person:
					return "Définitions de la personne";

				default:
					return null;
			}
		}
	}
}
