//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModuleLayer</c> enumeration defines the well-known layers
	/// found in a multi-layered application. See <see cref="ResourceModuleId"/>
	/// for support methods.
	/// </summary>
	public enum ResourceModuleLayer
	{
		Undefined,
		
		System,

		
		Application,
		
		Customization1,
		Customization2,
		Customization3,

		User,
	}
}
