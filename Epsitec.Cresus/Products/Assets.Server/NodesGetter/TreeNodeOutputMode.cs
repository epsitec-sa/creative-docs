//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public enum TreeNodeOutputMode
	{
		All,				// tous les noeuds
		OnlyDescendants,	// tous les descendants
		OnlyParents,		// seulement les parents directs (pas les grands-parents)
		OnlyFinals,			// seulement les enfants
	}
}