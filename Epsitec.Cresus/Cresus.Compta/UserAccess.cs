//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public enum UserAccess
	{
		None            = 0x00000000,
		Full            = 0x0fffffff,
					    
		Admin           = 0x00000001,

		//	Présentations accessibles:
		Réglages        = 0x00000010,
		Utilisateurs    = 0x00000020,
		PiècesGenerator = 0x00000040,
		Libellés        = 0x00000080,
		Modèles         = 0x00000100,
		Journaux        = 0x00000200,
		Périodes        = 0x00000400,
		PlanComptable   = 0x00000800,
	}
}
