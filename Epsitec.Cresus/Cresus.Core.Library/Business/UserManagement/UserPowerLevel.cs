//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	/// <summary>
	/// The <c>UserPowerLevel</c> enumeration lists all supported power levels. The lowest
	/// value implies the highest power (except for the value <c>None</c>).
	/// </summary>
	[DesignerVisible]
	public enum UserPowerLevel
	{
		None			= 0,

		System			= 10,
		Developer		= 11,
		
		Administrator	= 20,

		PowerUser		= 30,
		Standard		= 40,
		Restricted		= 50,
	}
}
