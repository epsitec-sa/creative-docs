using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Données d'un résumé périodique.
	/// La structure est indentique à Exploitation, mais c'est une autre logique.
	/// </summary>
	public class RésuméPériodique : AbstractRecord
	{
		public Compte			Compte;
		public decimal?			Solde;
	}
}
