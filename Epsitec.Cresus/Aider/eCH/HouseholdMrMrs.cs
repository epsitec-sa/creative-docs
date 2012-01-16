//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.eCH
{
	[DesignerVisible]
	public enum HouseholdMrMrs
	{
		None = 0,
		Auto = 1,

		MonsieurEtMadame = 10,
		MadameEtMonsieur = 11,
		Famille = 12,
	}
}
