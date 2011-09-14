//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	[System.Flags]
	public enum RoundingPolicy
	{
		None					= 0,

		OnUnitPrice				= 0x0002,
		OnLinePrice				= 0x0004,

		OnTotalRounding			= 0x0010,
		OnTotalPrice			= 0x0020,
		OnTotalVat				= 0x0040,
		OnEndTotal				= 0x0080,
		
		All						= 0x00ff,
	}
}