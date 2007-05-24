//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[DesignerVisible]
	public enum BindingMode : byte
	{
		None,

		OneTime,								//	copy data from source to target, once
		OneWay,									//	copy data from source to target
		OneWayToSource,							//	copy data from target to source

		TwoWay,									//	copy data in both directions
		
//		UseDefaultValue,
	}
}
