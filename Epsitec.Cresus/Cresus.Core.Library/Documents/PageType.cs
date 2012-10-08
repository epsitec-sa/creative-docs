//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	/// <summary>
	/// The <c>PageType</c> enumeration lists all different types of pages which can
	/// then be mapped to specific printing units.
	/// </summary>
	[DesignerVisible]
	public enum PageType
	{
		//	ATTENTION: Les noms des pages sont sérialisés. Il ne faut donc pas les changer !

		Unknown		= 0,

		All			= 1,	// toutes les pages
		Copy		= 2,	// copie de toutes les pages

		Single		= 3,	// une page unique
		First		= 4,	// la première page (0)
		Following	= 5,	// les pages suivantes (1..n)

		Isr			= 6,	// BV
		Label		= 7,	// étiquettes
	}
}
