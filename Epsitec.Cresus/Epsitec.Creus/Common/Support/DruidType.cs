//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>DruidType</c> enumeration defines the type of a DRUID.
	/// </summary>
	public enum DruidType
	{
		/// <summary>
		/// The DRUID is invalid; it does not reference any resource.
		/// </summary>
		Invalid,
		
		/// <summary>
		/// The DRUID provides a module relative resource reference. It only
		/// encodes the developer id and local id (44-bit version of the DRUID
		/// value).
		/// </summary>
		ModuleRelative,
		
		/// <summary>
		/// The DRUID provided a full resource reference. It encodes the module
		/// id, the developer id and the local id (64-bit version of the DRUID
		/// value).
		/// </summary>
		Full
	}
}
