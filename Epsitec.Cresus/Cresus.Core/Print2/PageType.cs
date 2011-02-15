//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2
{
	public enum PageType
	{
		//	ATTENTION: Les noms des pages sont sérialisés. Il ne faut donc pas les changer !

		Unknown,

		All,			// toutes les pages
		Copy,			// copie de toutes les pages

		Single,			// une page unique
		First,			// la première page (0)
		Following,		// les pages suivantes (1..n)

		Esr,			// BV
		Label,			// étiquettes
	}
}
