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
				case SettingsEnum.DecimalDigits0:
					return "0";

				case SettingsEnum.DecimalDigits1:
					return "1";

				case SettingsEnum.DecimalDigits2:
					return "2";

				case SettingsEnum.DecimalDigits3:
					return "3";

				case SettingsEnum.DecimalDigits4:
					return "4";

				case SettingsEnum.DecimalDigits5:
					return "5";


				case SettingsEnum.YearDigits2:
					return "2";

				case SettingsEnum.YearDigits4:
					return "4";


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

				default:
					return TextFormatter.FormatText (type).ApplyFontColor (Color.FromName ("Red"));
			}
		}
	}
}
