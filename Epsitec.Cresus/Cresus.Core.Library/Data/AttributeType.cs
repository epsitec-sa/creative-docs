//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	[DesignerVisible]
	public enum AttributeType
	{
		Unknown		= -1,
		None		= 0,

		Flag		= 100,

		Event		= 200,

		Task		= 300,
	}
}
