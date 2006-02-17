//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public enum BindingMode
	{
		None,

		OneTime,
		OneWay,
		OneWayToSource,
		
		TwoWay,
		
		UseDefaultValue,
	}
}
