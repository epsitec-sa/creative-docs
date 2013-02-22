using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class RésuméTva : AbstractRecord
	{
		public bool				LigneEnTête;
		public bool				LigneDeTotal;
		public Compte			Compte;
		public FormattedText	CodeTVA;
		public decimal?			Taux;
		public Date?			Date;
		public FormattedText	Pièce;
		public FormattedText	Titre;
		public decimal?			Montant;
		public decimal?			TVA;
		public decimal?			Différence;
	}
}
