//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>DiscountPolicy</c> enumeration defines on what prices a discount should be
	/// applied.
	/// </summary>
	[DesignerVisible]
	public enum DiscountPolicy
	{
		None					= 0,
		All						= 1,

		OnUnitPrice				= 2,
		OnLinePrice				= 4,
	}
}
