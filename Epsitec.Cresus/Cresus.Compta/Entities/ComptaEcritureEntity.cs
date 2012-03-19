//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaEcritureEntity
	{
		public bool IsEmptyLine
		{
			//	Retourne true s'il s'agit d'une ligne additionnelle vide. Ces lignes sont proposées
			//	lors de la sélection d'une écriture multiple, pour permettre à l'utilisateur de
			//	facilement créer une nouvelle ligne, simplement en la remplissant. S'il la laisse
			//	vide, elle est ignorée.
			get
			{
				return this.Type == (int) TypeEcriture.Vide &&
					   this.Débit == null &&
					   this.Crédit == null &&
					   this.Pièce.IsNullOrEmpty &&
					   this.Libellé.IsNullOrEmpty &&
					   this.Montant == 0;
			}
		}


		public FormattedText ShortType
		{
			get
			{
				return ComptaEcritureEntity.GetShortType ((TypeEcriture) this.Type);
			}
		}

		public static FormattedText GetShortType(TypeEcriture type)
		{
			switch ((TypeEcriture) type)
			{
				case TypeEcriture.Nouveau:
					return "N";

				case TypeEcriture.Vide:
					return "V";

				case TypeEcriture.BaseTVA:
					return "T";

				case TypeEcriture.CodeTVA:
					return "C";

				case TypeEcriture.Arrondi:
					return "A";

				case TypeEcriture.Escompte:
					return "E";

				default:
					return null;
			}
		}


		public FormattedText ShortLibelléTVA
		{
			//	Retourne le libellé court, avec juste les détails de la TVA, ou rien s'il s'agit d'une écriture sans TVA.
			get
			{
				if (this.CodeTVA == null)
				{
					return FormattedText.Empty;
				}
				else
				{
					return ComptaEcritureEntity.GetLibelléTVA (this.CodeTVA.Code, this.TauxTVA);
				}
			}
		}

		public FormattedText FullLibelléTVA
		{
			//	Retourne le libellé complet, avec les détails de la TVA s'ils existent.
			get
			{
				if (this.CodeTVA == null)
				{
					return this.Libellé;
				}
				else
				{
					return FormattedText.Concat (this.Libellé, ", ", ComptaEcritureEntity.GetLibelléTVA (this.CodeTVA.Code, this.TauxTVA));
				}
			}
		}

		private static FormattedText GetLibelléTVA(FormattedText code, decimal? taux)
		{
			//	Retourne par exemple "TVA 8.0% (IPM)".
			if (!code.IsNullOrEmpty && taux.HasValue)
			{
				return string.Format ("TVA {0} ({1})", Converters.PercentToString (taux), code);
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return Core.TextFormatter.FormatText (this.Date, this.Débit.Numéro, this.Crédit.Numéro, this.Pièce, this.Libellé, this.Montant.ToString ());
		}
	}
}
