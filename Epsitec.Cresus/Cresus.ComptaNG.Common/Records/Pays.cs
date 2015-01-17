using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	public class Pays : AbstractRecord
	{
		public FormattedText	Description;
		public FormattedText	Commentaire;  // utile ?
		public string			CodeIso;
	}
}
