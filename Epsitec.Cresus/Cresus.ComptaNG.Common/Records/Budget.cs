using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class Budget : AbstractRecord
	{
		public PériodeComptable		PériodeComptable;
		public decimal				Montant;
		public FormattedText		Description;
		public FormattedText		Commentaire;  // utile ?
	}
}
