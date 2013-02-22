//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	[DesignerVisible]
	public enum SwissPostZipType
	{
		None				= 0,
		Mixed				= 10,
		DomicileOnly		= 20,
		POBoxOnly			= 30,
		Company				= 40,
		Internal			= 80,
	}
}
