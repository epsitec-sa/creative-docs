using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Data
{
	/// <summary>
	/// Cette classe reprend mon implémentation assez complexe de Epsitec.Cresus.Compta.IO.PiècesGenerator,
	/// avec numéros brûlés, congélateur, etc.
	/// Il faudra voir si l'aspect multi utilisateur n'implique pas quelque chose de beaucoup moins
	/// souple.
	/// </summary>
	public class GénérateurDePièces : AbstractObjetComptable
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
