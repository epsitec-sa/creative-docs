//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleLayer</c> enumeration defines the well-known layers
	/// found in a multi-layered application. See <see cref="ResourceModuleInfo"/>
	/// for support methods.
	/// </summary>
	public enum ResourceModuleLayer
	{
		Undefined,

		Application,
		
		Customization1,
		Customization2,
		Customization3,

		User,
	}
}
