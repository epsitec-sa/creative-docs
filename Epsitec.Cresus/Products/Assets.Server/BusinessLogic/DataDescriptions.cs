//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class DataDescriptions
	{
		public static string GetObjectFieldDescription(ObjectField field)
		{
			if (field >= ObjectField.GroupGuidRatioFirst &&
				field <= ObjectField.GroupGuidRatioLast)
			{
				return "Dans";
			}

			switch (field)
			{
				case ObjectField.OneShotNumber:
					return "Evénement numéro";

				case ObjectField.OneShotDateOperation:
					return "Date opération";

				case ObjectField.OneShotComment:
					return "Commentaire";

				case ObjectField.OneShotDocuments:
					return "Documents associés";

				case ObjectField.GroupParent:
					return "Dans le groupe";

				case ObjectField.Number:
					return "Numéro";

				case ObjectField.Name:
					return "Nom";

				case ObjectField.Description:
					return "Description";

				case ObjectField.MainValue:
					return "Valeur comptable";

				case ObjectField.CategoryName:
					return "Catégorie d'immob.";

				case ObjectField.AmortizationRate:
					return "Taux";

				case ObjectField.AmortizationType:
					return "Type";

				case ObjectField.Periodicity:
					return "Périodicité";

				case ObjectField.Prorata:
					return "Prorata";

				case ObjectField.Round:
					return "Arrondi";

				case ObjectField.ResidualValue:
					return "Valeur résiduelle";

				case ObjectField.Account1:
					return "Contrepartie achat";

				case ObjectField.Account2:
					return "Contrepartie vente";

				case ObjectField.Account3:
					return "Compte d'immob.";

				case ObjectField.Account4:
					return "Compte d'amort.";

				case ObjectField.Account5:
					return "Charge d'amort.";

				case ObjectField.Account6:
					return "Compte réévaluation";

				case ObjectField.Account7:
					return "";

				case ObjectField.Account8:
					return "";

				case ObjectField.UserFieldType:
					return "Type";

				case ObjectField.UserFieldColumnWidth:
					return "Largeur colonne";

				case ObjectField.UserFieldLineWidth:
					return "Largeur ligne";

				case ObjectField.UserFieldLineCount:
					return "Nombre de lignes";

				case ObjectField.UserFieldTopMargin:
					return "Marge supérieure";

				case ObjectField.UserFieldSummaryOrder:
					return "Ordre dans résumé";

				case ObjectField.AccountCategory:
					return "Catégorie";

				case ObjectField.AccountType:
					return "Type";

				case ObjectField.EntryDate:
					return "Date";

				case ObjectField.EntryDebitAccount:
					return "Débit";

				case ObjectField.EntryCreditAccount:
					return "Crédit";

				case ObjectField.EntryStamp:
					return "Pièce";

				case ObjectField.EntryTitle:
					return "Libellé";

				case ObjectField.EntryAmount:
					return "Montant";

				default:
					return null;
			}
		}

		public static string GetEventDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Input:
					return "Entrée";

				case EventType.AmortizationPreview:
					return "Aperçu amort.";

				case EventType.AmortizationAuto:
					return "Amort. ordinaire";

				case EventType.AmortizationExtra:
					return "Amort. extraordinaire";

				case EventType.Modification:
					return "Modification générale";

				case EventType.MainValue:
					return "Modification valeur";

				case EventType.Output:
					return "Sortie";

				default:
					return null;
			}
		}
	}
}
