using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Compta.NouvelleStructure
{
	public class GénérateurPièce : ObjetComptable
	{
		public FormattedText	Nom;
		public FormattedText	Préfixe;
		public FormattedText	Suffixe;
		public FormattedText	Format;
		public int				Numéro;
		public int				Incrément;
		public FormattedText	Commentaire;  // utile ?
	}
}
