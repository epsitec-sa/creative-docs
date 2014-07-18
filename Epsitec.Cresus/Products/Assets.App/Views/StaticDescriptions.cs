//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetViewTypeIcon(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return "View.Assets";

				case ViewTypeKind.Amortizations:
					return "View.Amortizations";

				case ViewTypeKind.Entries:
					return "View.Ecritures";

				case ViewTypeKind.Categories:
					return "View.Categories";

				case ViewTypeKind.Groups:
					return "View.Groups";

				case ViewTypeKind.Persons:
					return "View.Persons";

				case ViewTypeKind.Reports:
					return "View.Reports";

				case ViewTypeKind.Warnings:
					return "View.Warnings";

				case ViewTypeKind.AssetsSettings:
					return "View.AssetsSettings";

				case ViewTypeKind.PersonsSettings:
					return "View.PersonsSettings";

				case ViewTypeKind.Accounts:
					return "View.AccountsSettings";

				default:
					return null;
			}
		}

		public static string GetViewTypeDescription(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return "Objets d'immobilisation";

				case ViewTypeKind.Amortizations:
					return "Amortissements";

				case ViewTypeKind.Entries:
					return "Ecritures comptables";

				case ViewTypeKind.Categories:
					return "Catégories d'immobilisations";

				case ViewTypeKind.Groups:
					return "Groupes";

				case ViewTypeKind.Persons:
					return "Contacts";

				case ViewTypeKind.Reports:
					return "Rapports et statistiques";

				case ViewTypeKind.Warnings:
					return "Avertissements";

				case ViewTypeKind.AssetsSettings:
					//?return "Réglages — Champs des objets d'immobilisations";
					return "Champs des objets d'immobilisations";

				case ViewTypeKind.PersonsSettings:
					//?return "Réglages — Champs des contacts";
					return "Champs des contacts";

				case ViewTypeKind.Accounts:
					//?return "Réglages — Plan comptable";
					return "Plan comptable";

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

				case PageType.Asset:
					return "Général";

				case PageType.AmortizationValue:
					return "Valeur";

				case PageType.AmortizationDefinition:
					return "Amortissement";

				case PageType.Groups:
					return "Groupes";

				case PageType.Category:
					return "Définitions de la catégorie";

				case PageType.Group:
					return "Définitions du groupe";

				case PageType.Person:
					return "Définitions du contact";

				case PageType.UserFields:
					return "Définitions du champ";

				case PageType.Account:
					return "Définitions du compte";

				default:
					return null;
			}
		}
	}
}
