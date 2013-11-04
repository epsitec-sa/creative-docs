//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetObjectFieldDescription(ObjectField field)
		{
			switch (field)
			{
				case ObjectField.EvNuméro:
					return "Evénement numéro";

				case ObjectField.EvCommentaire:
					return "Commentaire";

				case ObjectField.EvDocuments:
					return "Documents associés";

				case ObjectField.Level:
					return "Niveau";

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

		public static string GetObjectPageDescription(EditionObjectPageType type)
		{
			switch (type)
			{
				case EditionObjectPageType.Singleton:
					return "Evénement";

				case EditionObjectPageType.Summary:
					return "Résumé";

				case EditionObjectPageType.General:
					return "Général";

				case EditionObjectPageType.Values:
					return "Valeurs";

				case EditionObjectPageType.Amortissements:
					return "Amortissements";

				case EditionObjectPageType.Category:
					return "Général";

				case EditionObjectPageType.Compta:
					return "Comptabilisation";

				default:
					return null;
			}
		}
	}
}
