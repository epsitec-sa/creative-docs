//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider
{
	[DesignerVisible]
	public enum ValidationState
	{
		Undefined = 0,

		Valid = 1,
		Pending = 2,
		Invalid = 3,
		Deleted = 4,
	}
}
