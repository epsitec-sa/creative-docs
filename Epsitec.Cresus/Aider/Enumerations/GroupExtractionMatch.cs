//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum GroupExtractionMatch
	{
		Path				= 0,	//	path and wildcards specified manually
		
		AnyRegionAnyParish	= 1,	//	R___.P___.
		OneRegionAnyParish	= 2,	//	Rxxx.P___.
		
		SameFunction		= 100,	//	matches groups which share the same GroupDef.Function
	}
}

