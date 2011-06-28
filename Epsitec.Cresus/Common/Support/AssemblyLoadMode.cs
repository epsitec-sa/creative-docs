//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>AssemblyLoadMode</c> enumeration is used by the <see cref="AssemblyLoader"/>
	/// to decide which assemblies may (or may not) be loaded.
	/// </summary>
	public enum AssemblyLoadMode
	{
		LoadAny,
		LoadOnlySigned,
		LoadOnlyEpsitecSigned,
	}
}
