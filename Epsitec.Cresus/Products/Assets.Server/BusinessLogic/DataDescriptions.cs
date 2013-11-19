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

				case ObjectField.Parent:
					return "Dans le groupe";

				case ObjectField.Numéro:
					return "Numéro";

				case ObjectField.Nom:
					return "Nom";

				case ObjectField.Description:
					return "Description";

				case ObjectField.Valeur1:
					return "Valeur comptable";

				case ObjectField.Valeur2:
					return "Valeur d'assurance";

				case ObjectField.Valeur3:
					return "Valeur imposable";

				case ObjectField.NomCatégorie1:
				case ObjectField.NomCatégorie2:
				case ObjectField.NomCatégorie3:
					return "Catégorie d'immob.";

				case ObjectField.Responsable:
					return "Responsable";

				case ObjectField.Couleur:
					return "Couleur";

				case ObjectField.NuméroSérie:
					return "Numéro de série";

				case ObjectField.DateAmortissement1:
					return "Date 1er amort.";

				case ObjectField.DateAmortissement2:
					return "Date 2ème amort.";

				case ObjectField.TauxAmortissement:
					return "Taux";

				case ObjectField.TypeAmortissement:
					return "Type";

				case ObjectField.FréquenceAmortissement:
					return "Fréquence";

				case ObjectField.Périodicité:
					return "Périodicité";

				case ObjectField.ValeurRésiduelle:
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

				case ObjectField.Famille:
					return "Famille";

				case ObjectField.GroupGuid+0:
				case ObjectField.GroupGuid+1:
				case ObjectField.GroupGuid+2:
				case ObjectField.GroupGuid+3:
				case ObjectField.GroupGuid+4:
				case ObjectField.GroupGuid+5:
				case ObjectField.GroupGuid+6:
				case ObjectField.GroupGuid+7:
				case ObjectField.GroupGuid+8:
				case ObjectField.GroupGuid+9:
					return "Dans le groupe";

				case ObjectField.GroupRate+0:
				case ObjectField.GroupRate+1:
				case ObjectField.GroupRate+2:
				case ObjectField.GroupRate+3:
				case ObjectField.GroupRate+4:
				case ObjectField.GroupRate+5:
				case ObjectField.GroupRate+6:
				case ObjectField.GroupRate+7:
				case ObjectField.GroupRate+8:
				case ObjectField.GroupRate+9:
					return "Au taux de";

				default:
					return null;
			}
		}

		public static string GetEventDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Entrée:
					return "Entrée";

				case EventType.AmortissementAuto:
					return "Amort. ordinaire";

				case EventType.AmortissementExtra:
					return "Amort. extraordinaire";

				case EventType.Modification:
					return "Modification";

				case EventType.Réorganisation:
					return "Réorganisation";

				case EventType.Augmentation:
					return "Augmentation";

				case EventType.Diminution:
					return "Diminution";

				case EventType.Sortie:
					return "Sortie";

				default:
					return null;
			}
		}


		public readonly static string OutOfDateName = "<i>Inconnu à cette date</i>";
	}
}
