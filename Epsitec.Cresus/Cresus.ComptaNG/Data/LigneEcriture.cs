using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// "Ligne" d'une écriture simple ou multiple, pouvant générér de la Tva.
	/// Sans Tva, une "ligne" d'écriture est représentée en une seule ligne.
	/// Avec Tva, une "ligne" d'écriture est habituellement représentée en 3 lignes.
	/// Par exemple:
	///   - Débit:        4200
	///   - Crédit:       1000
	///   - Libellé:      Achat pièce
	///   - Montant:      200.00
	///   - MontantTVA:    15.20
	///   - MontantTotal: 215.20
	///   - CodeTva:      IPM
	///   - TauxTva:      7.6%
	///   - TvaAuDébit:   true
	/// Affichage souhaité:
	///   - 4200 ...  Achat pièces, (IPM) net, TVA = 15.20  200.00
	///   - 1170 ...  Achat pièces, 7.6% de TVA (IPM)        15.20
	///   - ...  1000 Achat pièces Total, (IPM)             215.20
	/// </summary>
	public class LigneEcriture : AbstractObjetComptable
	{
		public DateTime			DateComptable;
		public Compte			Débit;
		public Compte			Crédit;
		public FormattedText	Pièce;
		public FormattedText	Libellé;
		public FormattedText	Commentaire;  // utile ?
		public decimal			Montant;
		public decimal			MontantTVA;
		public decimal			MontantTotal;
		public CodeTva			CodeTVA;
		public decimal			TauxTVA;
		public bool				TvaAuDébit;
	}
}
