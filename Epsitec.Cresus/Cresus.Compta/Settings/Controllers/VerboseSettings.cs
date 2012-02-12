//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public static class VerboseSettings
	{
		public static FormattedText GetDescription(string name)
		{
			switch (name)
			{
				case "Global":
					return "Général";

				case "Global.Titre":
					return "Titre de la comptabilité";

				case "Global.Description":
					return "Description de la comptabilité";


				case "Ecriture":
					return "Journal des écritures";

				case "Ecriture.Pièces":
					return "Ecritures avec numéros de pièce";

				case "Ecriture.AutoPièces":
					return "Propose automatiquement un numéro de pièce";

				case "Ecriture.ProchainePièce":
					return "Numéro de la prochaine pièce";

				case "Ecriture.IncrémentPièce":
					return "Incrément automatique pour la pièce";

				case "Ecriture.PlusieursPièces":
					return "Numéro de pièce individuel pour chaque ligne d'une écriture multiple";

				case "Ecriture.ForcePièces":
					return "Force un numéro de pièce non modifiable";


				case "Price":
					return "Format des montants";

				case "Price.DecimalDigits":
					return "Nombre de décimales";

				case "Price.DecimalSeparator":
					return "Séparateur de la partie fractionnaire";

				case "Price.GroupSeparator":
					return "Séparateur des milliers";

				case "Price.NullParts":
					return "Affichage des parties nulles";

				case "Price.NegativeFormat":
					return "Nombres négatifs";

				case "Price.Sample":
					return "Exemples";


				case "Date":
					return "Format des dates";

				case "Date.Separator":
					return "Séparateur";

				case "Date.Year":
					return "Nombre de chiffres pour l'année";

				case "Date.Order":
					return "Ordre des 3 éléments constitutifs";

				case "Date.Sample":
					return "Exemples";


				case ".":
					return "Point";

				case ",":
					return "Virgule";

				case "'":
					return "Apostrophe";

				case "-":
					return "Tiret";

				case "/":
					return "Barre oblique";

				case "Space":
					return "Espace";

				case "None":
					return "Aucun";


				case "Negative":
					return "-450";

				case "()":
					return "(450)";


				case "00":
					return "0.00";

				case "0t":
					return "0.—";

				case "t0":
					return "-.00";

				case "tt":
					return "-.—";

				case "0":
					return "0";


				case "DMY":
					return "jour mois année";

				case "YMD":
					return "année mois jour";


				case "1":
					return "1";

				case "2":
					return "2";

				case "3":
					return "3";

				case "4":
					return "4";

				case "5":
					return "5";


				default:
					return TextFormatter.FormatText (name).ApplyFontColor (Color.FromName ("Red"));
			}
		}
	}
}
