//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.Comptabilité
{
	[DesignerVisible]
	public enum CatégorieDeCompte
	{
		Inconnu			= 0,
		Actif			= 1,
		Passif			= 2,
		Charge			= 3,
		Produit			= 4,
		Exploitation	= 5,
	}
}
