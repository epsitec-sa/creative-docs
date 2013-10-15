//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetObjectFieldDescription(ObjectField field)
		{
			switch (field)
			{
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

				case ObjectField.Responsable:
					return "Responsable";

				case ObjectField.Couleur:
					return "Couleur";

				case ObjectField.NuméroSérie:
					return "Numéro de série";

				case ObjectField.NomCatégorie:
					return "Nom de la catégorie";

				case ObjectField.TauxAmortissement:
					return "Taux";

				case ObjectField.TypeAmortissement:
					return "Type";

				case ObjectField.FréquenceAmortissement:
					return "Fréquence d'amortissement";

				case ObjectField.ValeurRésiduelle:
					return "Valeur résiduelle";

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
					return "Amortissement ordinaire";

				case EventType.AmortissementExtra:
					return "Amortissement extraordinaire";

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
