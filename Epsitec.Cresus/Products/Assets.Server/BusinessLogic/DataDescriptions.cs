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
			switch (field)
			{
				case ObjectField.OneShotNuméro:
					return "Evénement numéro";

				case ObjectField.OneShotDateOpération:
					return "Date opération";

				case ObjectField.OneShotCommentaire:
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

				case ObjectField.Value1:
					return "Valeur d'assurance";

				case ObjectField.Value2:
					return "Valeur imposable";

				case ObjectField.Value3:
				case ObjectField.Value4:
				case ObjectField.Value5:
				case ObjectField.Value6:
				case ObjectField.Value7:
				case ObjectField.Value8:
				case ObjectField.Value9:
				case ObjectField.Value10:
					return "Valeur libre";

				case ObjectField.CategoryName:
					return "Catégorie d'immob.";

				case ObjectField.Maintenance:
					return "Maintenance";

				case ObjectField.Color:
					return "Couleur";

				case ObjectField.SerialNumber:
					return "Numéro de série";

				case ObjectField.Person1:
					return "Responsable";

				case ObjectField.Person2:
					return "Fournisseur";

				case ObjectField.Person3:
					return "Maintenance";

				case ObjectField.Person4:
					return "Concierge";

				case ObjectField.Person5:
					return "Conseiller";

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

				case ObjectField.GroupGuidRatio+0:
				case ObjectField.GroupGuidRatio+1:
				case ObjectField.GroupGuidRatio+2:
				case ObjectField.GroupGuidRatio+3:
				case ObjectField.GroupGuidRatio+4:
				case ObjectField.GroupGuidRatio+5:
				case ObjectField.GroupGuidRatio+6:
				case ObjectField.GroupGuidRatio+7:
				case ObjectField.GroupGuidRatio+8:
				case ObjectField.GroupGuidRatio+9:
					return "Dans le groupe";

				case ObjectField.Title:
					return "Titre";

				case ObjectField.Company:
					return "Entreprise";

				case ObjectField.FirstName:
					return "Prénom";

				case ObjectField.Address:
					return "Adresse";

				case ObjectField.Zip:
					return "NPA";

				case ObjectField.City:
					return "Ville";

				case ObjectField.Country:
					return "Pays";

				case ObjectField.Phone1:
					return "Téléphone prof.";

				case ObjectField.Phone2:
					return "Téléphone privé";

				case ObjectField.Phone3:
					return "Téléphone portable";

				case ObjectField.Mail:
					return "Adresse e-mail";

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
