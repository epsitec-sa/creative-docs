using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class PériodeComptable : AbstractRecord
	{
		public IntervalleDates			Dates;
		public FormattedText			Description;
		public FormattedText			Commentaire;  // utile ?
		public List<AbstractEcriture>	Ecritures;
		public DateTime					DernièreDate;
		public GénérateurDePièces		GénérateurDePièces;
	}
}
