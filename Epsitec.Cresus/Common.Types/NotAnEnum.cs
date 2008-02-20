//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>NotAnEnum</c> enumeration is used when an <see cref="T:IEnumValue"/>
	/// needs to return a <see cref="T:System.Enum"/> value, but the underlying type
	/// does not define a C# enum value for it.
	/// </summary>
	public enum NotAnEnum
	{
		/// <summary>
		/// Use <c>NotAnEnum.Instance</c> whenever you need to pass a <see cref="T:System.Enum"/>
		/// value, but none is valid in that context.
		/// </summary>
		Instance
	}
}
