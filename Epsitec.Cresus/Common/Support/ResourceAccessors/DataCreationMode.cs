//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>DataCreationMode</c> enumeration defines the possible modes
	/// used by the <c>FillDataFromCaption</c> method, when creating a
	/// <see cref="StructuredData"/> instance.
	/// </summary>
	public enum DataCreationMode
	{
		/// <summary>
		/// Creates a public data record, with attached event handlers.
		/// </summary>
		Public,
		
		/// <summary>
		/// Creates a lightweight data record, which should just be used
		/// to store temporary data. No event handlers get created for it
		/// and the data may be simply disposed of without any side effects.
		/// </summary>
		Temporary
	}
}
