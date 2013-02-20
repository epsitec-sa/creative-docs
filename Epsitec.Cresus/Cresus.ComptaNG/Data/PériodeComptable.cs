using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	public class PériodeComptable : ObjetComptable
	{
		public IntervalleDates			Dates;
		public FormattedText			Description;
		public FormattedText			Commentaire;  // utile ?
		public List<AbstractEcriture>	Ecritures;
		public DateTime					DernièreDate;
		public GénérateurPièce			GénérateurPièce;
	}
}
