//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.eCH
{
	[DesignerVisible]
	public enum RemovalReason
	{
		None = 0,

		Unknown,

		Deleted,
		Departed,
		Deceased,
	}
}
