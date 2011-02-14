//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2.Verbose
{
	public enum DocumentOptionValueType
	{
		Undefined,
		Boolean,		// valeur "true" ou "false"
		Distance,		// type 'double' correspondant à une distance en mm
	}
}
