//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.ModuleRepository
{
	/// <summary>
	/// The <c>ModuleState</c> enumeration defines the possible states for
	/// a given module.
	/// </summary>
	public enum ModuleState
	{
		Undefined,
		InUse,
		FreeForReuse,
	}
}
