using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class ExtraitDeCompte : AbstractRecord
	{
		public Date				Date;
		public Compte			CP;  // compte de la contre-partie
		public bool				CompteAuDébit;
		public FormattedText	Pièce;
		public FormattedText	Libellé;
		public FormattedText	LibelléTvaComplet;
		public decimal?			MontantAuDébit;
		public decimal?			MontantAuCrédit;
		public decimal?			Solde;
		public CodeTva			CodeTVA;
		public decimal?			TauxTVA;
		public Journal			Journal;
	}
}
