using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	public class EcritureModèle : ObjetComptable
	{
		public string			Code;
		public string			Raccourci;
		public Compte			Débit;
		public Compte			Crédit;
		public FormattedText	Pièce;
		public FormattedText	Libellé;
		public FormattedText	Commentaire;  // utile ?
		public decimal			Montant;
	}
}
