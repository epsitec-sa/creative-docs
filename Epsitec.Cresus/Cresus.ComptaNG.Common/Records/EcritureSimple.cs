using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Ecriture simple, qui ne contient trivialement d'une ligne.
	/// </summary>
	public class EcritureSimple : AbstractEcriture
	{
		public LigneEcriture Ecriture;
	}
}
