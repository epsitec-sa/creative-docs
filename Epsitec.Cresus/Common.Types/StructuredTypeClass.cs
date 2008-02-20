//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredTypeClass</c> enumeration is used to describe how a
	/// structured type should be interpreted.
	/// See <see cref="StructuredType"/>.
	/// </summary>
	[DesignerVisible]
	public enum StructuredTypeClass
	{
		/// <summary>
		/// The structured type should be considered as such: a simple structured
		/// type without any special semantic.
		/// </summary>
		None,

		/// <summary>
		/// The structured type defines an entity.
		/// </summary>
		Entity,

		/// <summary>
		/// The structured type defines an interface.
		/// </summary>
		Interface,
	}
}
