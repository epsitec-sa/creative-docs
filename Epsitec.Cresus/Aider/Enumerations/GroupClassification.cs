//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum GroupClassification
	{
		None			= 0,
		
		Function		= 10,
		Region			= 20,
		Parish			= 30,
		NoParish		= 40,
		DerogationIn	= 50,
		DerogationOut	= 60,
	}
}