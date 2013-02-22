using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Ecriture multiple composée d'une liste de lignes d'écritures.
	/// TotalAutomatique désigne une et une seule ligne dont le montant est
	/// la somme des autres lignes.
	/// </summary>
	public class EcritureMultiple : AbstractEcriture
	{
		public List<LigneEcriture>	Lignes;
		public LigneEcriture		TotalAutomatique;
		public FormattedText		Commentaire;  // utile ?
	}
}
