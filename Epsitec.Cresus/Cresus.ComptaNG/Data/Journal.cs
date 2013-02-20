using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	public class Journal : ObjetComptable
	{
		public FormattedText	Nom;
		public FormattedText	Description;
		public FormattedText	Commentaire;  // utile ?
		public GénérateurPièce	GénérateurPièce;
	}
}
