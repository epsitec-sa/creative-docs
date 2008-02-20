//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>UnresolvedEnum</c> enumeration is used when an <see cref="T:IEnumValue"/>
	/// needs to return a <see cref="T:System.Enum"/> value, but the underlying type
	/// cannot be resolved to a loaded assembly.
	/// </summary>
	public enum UnresolvedEnum
	{
		/// <summary>
		/// Use <c>UnresolvedEnum.Instance</c> whenever you need to pass a <see cref="T:System.Enum"/>
		/// value, but its type is not available because the referred assembly is missing.
		/// </summary>
		Instance
	}
}
