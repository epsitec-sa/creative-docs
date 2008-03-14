//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StringComparisonBehavior</c> enumeration defines how strings
	/// are compared for equality and for sorting. This is equivalent to the
	/// <see cref="System.StringComparison"/> enumeration.
	/// </summary>
	[DesignerVisible]
	public enum StringComparisonBehavior
	{
		Ordinal = System.StringComparison.Ordinal,
		OrdinalIgnoreCase = System.StringComparison.OrdinalIgnoreCase,
		CurrentCulture = System.StringComparison.CurrentCulture,
		CurrentCultureIgnoreCase = System.StringComparison.CurrentCultureIgnoreCase,
		InvariantCulture = System.StringComparison.InvariantCulture,
		InvariantCultureIgnoreCase = System.StringComparison.InvariantCultureIgnoreCase
	}
}
