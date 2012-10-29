//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>ColumnFilterSetCode</c> enumeration defines what predicate is used with a
	/// <see cref="ColumnFilterSetExpression"/>.
	/// </summary>
	public enum ColumnFilterSetCode
	{
		Undefined,

		In,
		NotIn,
	}
}