using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Taux de TVA avec sa date d'introduction. Une nouvelle instance est créée
	/// lors de l'introduction d'un nouveau taux par la confédération.
	/// L'historique des taux normaux est le suivant:
	///   - 01.01.1995 6.5%
	///   - 01.01.1999 7.5%
	///   - 01.01.2001 7.6%
	///   - 01.01.2011 8.0%
	/// L'historique des taux d'hébergement:
	///   - 01.01.1995 3.0%
	///   - 01.01.1999 3.5%
	///   - 01.01.2001 3.6%
	///   - 01.01.2011 3.8%
	/// L'historique des taux réduits:
	///   - 01.01.1995 2.0%
	///   - 01.01.1999 2.3%
	///   - 01.01.2001 2.4%
	///   - 01.01.2011 2.5%
	/// </summary>
	public class TauxTva : AbstractObjetComptable
	{
		public DateTime		DeteDébut;
		public decimal		Taux;
	}
}
