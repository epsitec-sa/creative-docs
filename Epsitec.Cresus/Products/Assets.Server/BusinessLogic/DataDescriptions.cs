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
				return "Dans le groupe";
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

				case ObjectField.Compte1:
					return "Bilan";

				case ObjectField.Compte2:
					return "Compte d'amortissement";

				case ObjectField.Compte3:
					return "Contrepartie acquisition";

				case ObjectField.Compte4:
					return "Contrepartie vente";

				case ObjectField.Compte5:
					return "Réévaluation";

				case ObjectField.Compte6:
					return "Revalorisation";

				case ObjectField.Compte7:
					return "Fonds d’amortissement";

				case ObjectField.Compte8:
					return "Amortissement extra. ou rééval.";

				//-case ObjectField.Title:
				//-	return "Titre";
				//-
				//-case ObjectField.Company:
				//-	return "Entreprise";
				//-
				//-case ObjectField.FirstName:
				//-	return "Prénom";
				//-
				//-case ObjectField.Address:
				//-	return "Adresse";
				//-
				//-case ObjectField.Zip:
				//-	return "NPA";
				//-
				//-case ObjectField.City:
				//-	return "Ville";
				//-
				//-case ObjectField.Country:
				//-	return "Pays";
				//-
				//-case ObjectField.Phone1:
				//-	return "Téléphone prof.";
				//-
				//-case ObjectField.Phone2:
				//-	return "Téléphone privé";
				//-
				//-case ObjectField.Phone3:
				//-	return "Téléphone portable";
				//-
				//-case ObjectField.Mail:
				//-	return "Adresse e-mail";

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
					return "Modification";

				case EventType.Reorganization:
					return "Réorganisation";

				case EventType.Increase:
					return "Augmentation";

				case EventType.Decrease:
					return "Diminution";

				case EventType.Output:
					return "Sortie";

				default:
					return null;
			}
		}
	}
}
