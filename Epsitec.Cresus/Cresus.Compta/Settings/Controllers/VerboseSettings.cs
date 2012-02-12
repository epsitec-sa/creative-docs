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


				case "Nombres":
					return "Format des nombres";

				case "Nombres.Décimales":
					return "Nombre de décimales";

				case "Nombres.SépFrac":
					return "Séparateur de la partie fractionnaire";

				case "Nombres.Milliers":
					return "Séparateur des milliers";

				case "Nombres.Nul":
					return "Affichage des parties nulles";

				case "Nombres.Négatif":
					return "Nombres négatifs";


				case "Dates":
					return "Format des dates";

				case "Dates.Sép":
					return "Séparateur";

				case "Dates.Année":
					return "Nombre de chiffres pour l'année";

				case "Dates.Ordre":
					return "Ordre des 3 éléments constitutifs";


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

				case "Espace":
					return "Espace";

				case "Aucun":
					return "Aucun";


				case "Nég":
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


				case "jma":
					return "jour mois année";

				case "amj":
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
