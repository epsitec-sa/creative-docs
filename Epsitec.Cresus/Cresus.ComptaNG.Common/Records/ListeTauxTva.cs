using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Historique d'un taux de TVA. Par exemple, la liste "Nornmal" contient ceci:
	///   - 01.01.1995 6.5%
	///   - 01.01.1999 7.5%
	///   - 01.01.2001 7.6%
	///   - 01.01.2011 8.0%
	/// Le taux par défaut est généralement le dernier.
	/// </summary>
	public class ListeTauxTva : AbstractRecord
	{
		public FormattedText	Nom;
		public List<TauxTva>	ListeTaux;
		public TauxTva			TauxParDéfaut;
		public FormattedText	Commentaire;  // utile ?
	}
}
