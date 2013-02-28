using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Exemple d'un code de TVA:
	///   - Code:        IPMRED
	///   - Description: Impôt préalable sur l'achat de marchandises et prestations de services au taux réduit
	///   - ListeTaux:   Réduit
	///   - Compte:      1170
	/// </summary>
	public class CodeTva : AbstractRecord
	{
		public string			Code;
		public FormattedText	Description;
		public FormattedText	Commentaire;  // utile ?
		public ListeTauxTva		ListeTaux;
		public Compte			Compte;
		public decimal			Déduction;  // utile ?
		public int				Chiffre;  // utile ?
		public bool				Désactivé;  // utile ?
		public Pays				Pays;
	}
}
