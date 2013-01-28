//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>EntitySaveMode</c> enumeration is used to specify how <see cref="BusinessContext"/>
	/// should save the entities.
	/// </summary>
	[System.Flags]
	public enum EntitySaveMode
	{
		None					= 0,

		IncludeEmpty			= 0x0001,
		IgnoreValidationErrors	= 0x0002,
	}
}
