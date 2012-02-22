//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		//	Pr�sentations accessibles:
		R�glages        = 0x00000010,
		Utilisateurs    = 0x00000020,
		Pi�cesGenerator = 0x00000040,
		Libell�s        = 0x00000080,
		Mod�les         = 0x00000100,
		Journaux        = 0x00000200,
		P�riodes        = 0x00000400,
		PlanComptable   = 0x00000800,
	}
}
