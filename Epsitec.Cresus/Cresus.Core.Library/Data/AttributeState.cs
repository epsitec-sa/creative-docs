//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	[DesignerVisible]
	public enum AttributeState
	{
		None = 0,

		Draft = 1,

		Active = 2,
		Inactive = 3,
	}
}
