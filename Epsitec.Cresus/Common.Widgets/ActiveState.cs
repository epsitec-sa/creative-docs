//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ActiveState</c> enumeration defines if a <see cref="Visual"/> is
	/// active or not.
	/// </summary>
	public enum ActiveState : byte
	{
		/// <summary>
		/// The visual is not active.
		/// </summary>
		No=0,

		/// <summary>
		/// The visual is active.
		/// </summary>
		Yes=1,
		
		/// <summary>
		/// The visual is in a special state, neither active, nor inactive.
		/// </summary>
		Maybe=2
	}
}
