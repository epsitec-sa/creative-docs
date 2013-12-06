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

				case ObjectField.ValeurComptable:
					return "Valeur comptable";

				case ObjectField.Valeur1:
					return "Valeur d'assurance";

				case ObjectField.Valeur2:
					return "Valeur imposable";

				case ObjectField.Valeur3:
				case ObjectField.Valeur4:
				case ObjectField.Valeur5:
				case ObjectField.Valeur6:
				case ObjectField.Valeur7:
				case ObjectField.Valeur8:
				case ObjectField.Valeur9:
				case ObjectField.Valeur10:
					return "Valeur libre";

				case ObjectField.NomCatégorie:
					return "Catégorie d'immob.";

				case ObjectField.Maintenance:
					return "Maintenance";

				case ObjectField.Couleur:
					return "Couleur";

				case ObjectField.NuméroSérie:
					return "Numéro de série";

				case ObjectField.TauxAmortissement:
					return "Taux";

				case ObjectField.TypeAmortissement:
					return "Type";

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

#if true
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
#else
				case ObjectField.GroupGuidRatio+0:
					return "Dans le groupe 0";
				case ObjectField.GroupGuidRatio+1:
					return "Dans le groupe 1";
				case ObjectField.GroupGuidRatio+2:
					return "Dans le groupe 2";
				case ObjectField.GroupGuidRatio+3:
					return "Dans le groupe 3";
				case ObjectField.GroupGuidRatio+4:
					return "Dans le groupe 4";
				case ObjectField.GroupGuidRatio+5:
					return "Dans le groupe 5";
				case ObjectField.GroupGuidRatio+6:
					return "Dans le groupe 6";
				case ObjectField.GroupGuidRatio+7:
					return "Dans le groupe 7";
				case ObjectField.GroupGuidRatio+8:
					return "Dans le groupe 8";
				case ObjectField.GroupGuidRatio+9:
					return "Dans le groupe 9";
#endif

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
	}
}
