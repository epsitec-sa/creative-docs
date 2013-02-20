using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Compta.NouvelleStructure
{
	public class Budget : ObjetComptable
	{
		public PériodeComptable		PériodeComptable;
		public decimal				Montant;
		public FormattedText		Description;
		public FormattedText		Commentaire;  // utile ?
	}
}
