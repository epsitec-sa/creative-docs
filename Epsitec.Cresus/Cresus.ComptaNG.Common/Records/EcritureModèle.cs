using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class EcritureModèle : AbstractRecord
	{
		public string			Code;
		public string			Raccourci;
		public FormattedText	Commentaire;  // utile ?
		public AbstractEcriture	Ecriture;
	}
}
