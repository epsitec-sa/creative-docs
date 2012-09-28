//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum GroupClassification
	{
		None = 0,

		Canton				= 10,
		Region				= 20,
		Parish				= 30,
		Common				= 40,
		External			= 50,

		Function			= 100,
		Staff				= 110,
		StaffAssociation	= 120,

	}
}