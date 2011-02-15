//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print
{
	public enum PreviewMode
	{
		Print,					// inmpression réelle

		PagedPreview,			// aperçu avant impression paginé
		ContinuousPreview,		// aperçu avant impression sur un ruban de hauteur infinie
	}
}
