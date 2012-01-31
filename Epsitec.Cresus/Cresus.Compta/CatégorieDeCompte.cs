//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	[DesignerVisible]
	public enum CatégorieDeCompte
	{
		Inconnu			= 0,
		Actif			= 0x01,
		Passif			= 0x02,
		Charge			= 0x04,
		Produit			= 0x08,
		Exploitation	= 0x10,
		Tous			= 0x1f,
	}
}
