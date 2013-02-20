using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Compta.NouvelleStructure
{
	/// <summary>
	/// Partie commune aux écritures simples et multiples.
	/// </summary>
	public class AbstractEcriture : ObjetComptable
	{
		public DateTime		DateDuJour;
		public Journal		Journal;
		public Monnaie		Monnaie;
	}
}
