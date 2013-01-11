//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Pdf.Array
{
	public enum ColumnType
	{
		Absolute,		// largeur absolue
		Automatic,		// largeur calculée selon le contenu
		Stretch,		// utilise le reste à disposition
	}
}
