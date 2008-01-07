//	Copyright � 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>SystemTypeFamily</c> enumeration classifies the <c>System.Type</c> types
	/// into families: classes, value types, enumerations and interfaces.
	/// </summary>
	public enum SystemTypeFamily
	{
		Unknown,

		Class,
		ValueType,
		Enum,
		Interface
	}
}
