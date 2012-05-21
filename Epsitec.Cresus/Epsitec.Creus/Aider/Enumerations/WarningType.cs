//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum WarningType
	{
		None = 0,

		Generic = 1,

		Mismatch = 1000,
		ParishMismatch = 1001,
	}
}
