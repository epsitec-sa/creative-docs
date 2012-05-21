//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CultureMapSource</c> enumeration lists all possible sources for
	/// the data found in a <see cref="CultureMap"/> instance.
	/// </summary>
	
	[DesignerVisible]
	public enum CultureMapSource : byte
	{
		/// <summary>
		/// Invalid source.
		/// </summary>
		[Hidden]
		Invalid,

		/// <summary>
		/// The data originates from a reference module.
		/// </summary>
		ReferenceModule,
		
		/// <summary>
		/// The data originates from a patch module.
		/// </summary>
		PatchModule,

		/// <summary>
		/// The data is the result of a merge operation between data coming
		/// both from a patch module and a reference module.
		/// </summary>
		DynamicMerge
	}
}
