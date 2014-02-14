//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetViewTypeIcon(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return "View.Objects";

				case ViewType.Amortizations:
					return "View.Amortizations";

				case ViewType.Categories:
					return "View.Categories";

				case ViewType.Groups:
					return "View.Groups";

				case ViewType.Persons:
					return "View.Persons";

				case ViewType.Events:
					return "View.Events";

				case ViewType.Reports:
					return "View.Reports";

				case ViewType.Settings:
					return "View.Settings";

				default:
					return null;
			}
		}

		public static string GetViewTypeDescription(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return "Objets d'immobilisation";

				case ViewType.Amortizations:
					return "Amortissements";

				case ViewType.Categories:
					return "Catégories d'immobilisations";

				case ViewType.Groups:
					return "Groupes";

				case ViewType.Persons:
					return "Personnes";

				case ViewType.Events:
					return "Evénements";

				case ViewType.Reports:
					return "Rapports et statistiques";

				case ViewType.Settings:
					return "Réglages";

				default:
					return null;
			}
		}

		public static string GetObjectPageDescription(PageType type)
		{
			switch (type)
			{
				case PageType.OneShot:
					return "Evénement";

				case PageType.Summary:
					return "Résumé";

				case PageType.Object:
					return "Général";

				case PageType.Persons:
					return "Personnes";

				case PageType.Values:
					return "Valeurs";

				case PageType.Amortization:
					return "Amortissement";

				case PageType.AmortizationPreview:
					return "Calcul de l'amortissement";

				case PageType.Groups:
					return "Regroupements";

				case PageType.Category:
					return "Définitions de la catégorie";

				case PageType.Group:
					return "Définitions du groupe";

				case PageType.Person:
					return "Définitions de la personne";

				default:
					return null;
			}
		}
	}
}
