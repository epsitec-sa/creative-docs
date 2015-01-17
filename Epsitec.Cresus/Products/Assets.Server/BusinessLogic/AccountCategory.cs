//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	[System.Flags]
	public enum AccountCategory
	{
		Unknown      = 0x0000,

		Actif        = 0x0001,
		Passif       = 0x0002,
		Charge       = 0x0004,
		Produit      = 0x0008,
		Exploitation = 0x0010,
		Revenu       = 0x0020,
		Depense      = 0x0040,
		Recette      = 0x0080,
	}
}