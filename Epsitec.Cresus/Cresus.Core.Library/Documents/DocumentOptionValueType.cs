//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents.Verbose
{
	public enum DocumentOptionValueType
	{
		Undefined,
		Enumeration,	// énumération libre
		Distance,		// type 'double' correspondant à une distance en mm
		Size,			// type 'double' correspondant à une dimension en mm
		Text,			// texte
		TextMultiline,	// texte multilignes
	}
}
