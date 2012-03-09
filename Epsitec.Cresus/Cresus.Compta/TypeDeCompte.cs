//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	[DesignerVisible]
	public enum TypeDeCompte
	{
		Inconnu = 0,
		Normal  = 1,
		Groupe  = 2,
		TVA     = 3,
	}
}
