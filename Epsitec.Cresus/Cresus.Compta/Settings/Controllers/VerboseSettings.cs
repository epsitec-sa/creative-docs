//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public static class VerboseSettings
	{
		public static FormattedText GetDescription(SettingsGroup group)
		{
			switch (group)
            {
            	case SettingsGroup.Global:
					return "Général";

            	case SettingsGroup.Ecriture:
					return "Journal des écritures";

				case SettingsGroup.Price:
					return "Format des montants";

				case SettingsGroup.Percent:
					return "Format des pourcentages";

				case SettingsGroup.Date:
					return "Format des dates";

				default:
					return TextFormatter.FormatText (group).ApplyFontColor (Color.FromName ("Red"));
            }
		}


		public static FormattedText GetDescription(SettingsType type)
		{
			switch (type)
			{
				case SettingsType.GlobalTitre:
					return "Titre de la comptabilité";

				case SettingsType.GlobalDescription:
					return "Description de la comptabilité";

				case SettingsType.GlobalRemoveConfirmation:
					return "Demande une confirmation avant de supprimer une ligne";


				case SettingsType.EcritureMontantZéro:
					return "Accepte des écritures avec un montant nul";

				case SettingsType.EcriturePièces:
					return "Ecritures avec numéros de pièce";

				case SettingsType.EcritureAutoPièces:
					return "Propose automatiquement un numéro de pièce";

				case SettingsType.EcriturePlusieursPièces:
					return "Numéro de pièce individuel pour chaque ligne d'une écriture multiple";

				case SettingsType.EcritureForcePièces:
					return "Force un numéro de pièce non modifiable";

				case SettingsType.EcritureMultiEditionLineCount:
					return "Nb lignes éditables d'une écriture multiple";

				case SettingsType.EcritureTVA:
					return "Gère la TVA";

				case SettingsType.EcritureEditeMontantTVA:
					return "Permet l'édition du montant de la TVA";

				case SettingsType.EcritureEditeMontantHT:
					return "Permet l'édition du montant HT";

				case SettingsType.EcritureEditeCompteTVA:
					return "Permet l'édition du compte de TVA";

				case SettingsType.EcritureEditeCodeTVA:
					return "Permet l'édition du code TVA";

				case SettingsType.EcritureEditeTauxTVA:
					return "Permet l'édition du taux de la TVA";

				case SettingsType.EcritureProposeVide:
					return "Propose des ligne vides pour compléter les écritures multiples";

				case SettingsType.EcritureMontreType:
					return "Montre le type de chaque ligne (debug)";

				case SettingsType.EcritureMontreOrigineTVA:
					return "Montre l'origine de la TVA (debug)";

				case SettingsType.EcritureArrondiTVA:
					return "Arrondi pour la TVA";


				case SettingsType.PriceDecimalDigits:
					return "Nombre de décimales";

				case SettingsType.PriceDecimalSeparator:
					return "Séparateur de la partie fractionnaire";

				case SettingsType.PriceGroupSeparator:
					return "Séparateur des milliers";

				case SettingsType.PriceNullParts:
					return "Affichage des parties nulles";

				case SettingsType.PriceNegativeFormat:
					return "Nombres négatifs";

				case SettingsType.PriceSample:
					return "Exemples";


				case SettingsType.PercentDecimalSeparator:
					return "Séparateur de la partie fractionnaire";

				case SettingsType.PercentFracFormat:
					return "Partie fractionnaire";

				case SettingsType.PercentSample:
					return "Exemples";


				case SettingsType.DateSeparator:
					return "Séparateur";

				case SettingsType.DateYear:
					return "Nombre de chiffres pour l'année";

				case SettingsType.DateOrder:
					return "Ordre des 3 composants";

				case SettingsType.DateSample:
					return "Exemples";


				default:
					return TextFormatter.FormatText (type).ApplyFontColor (Color.FromName ("Red"));
			}
		}


		public static FormattedText GetDescription(SettingsEnum type)
		{
			switch (type)
			{
				case SettingsEnum.YearDigits2:
					return string.Format ("2 ({0})", (Date.Today.Year%100).ToString ());

				case SettingsEnum.YearDigits4:
					return string.Format ("4 ({0})", Date.Today.Year.ToString ());


				case SettingsEnum.YearDMY:
					return "jour mois année";

				case SettingsEnum.YearYMD:
					return "année mois jour";


				case SettingsEnum.SeparatorNone:
					return "Aucun";

				case SettingsEnum.SeparatorSpace:
					return "Espace";

				case SettingsEnum.SeparatorDot:
					return "Point";

				case SettingsEnum.SeparatorComma:
					return "Virgule";

				case SettingsEnum.SeparatorSlash:
					return "Barre oblique";

				case SettingsEnum.SeparatorDash:
					return "Tiret";

				case SettingsEnum.SeparatorApostrophe:
					return "Apostrophe";


				case SettingsEnum.NegativeMinus:
					return "-450";

				case SettingsEnum.NegativeParentheses:
					return "(450)";


				case SettingsEnum.NullPartsZeroZero:
					return "0.00";

				case SettingsEnum.NullPartsDashZero:
					return "-.0";

				case SettingsEnum.NullPartsZeroDash:
					return "0.—";

				case SettingsEnum.NullPartsDashDash:
					return "-.—";


				case SettingsEnum.PercentFloating:
					return "Variable";

				case SettingsEnum.PercentFrac1:
					return "1 décimale";

				case SettingsEnum.PercentFrac2:
					return "2 décimales";

				case SettingsEnum.PercentFrac3:
					return "3 décimales";


				default:
					return TextFormatter.FormatText (type).ApplyFontColor (Color.FromName ("Red"));
			}
		}
	}
}
