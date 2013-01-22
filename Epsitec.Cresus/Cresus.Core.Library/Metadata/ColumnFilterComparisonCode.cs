//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Core.Metadata
{
	public enum ColumnFilterComparisonCode
	{
		Undefined,

		Equal,
		NotEqual,
		LessThan,
		GreaterThan,
		LessThanOrEqual,
		GreaterThanOrEqual,
		
		Contains,								//	LIKE %x%
		ContainsEscaped,
		NotContains,
		NotContainsEscaped,

		StartsWith,								//	LIKE x%
		StartsWithEscaped,
		NotStartsWith,
		NotStartsWithEscaped,
	}
}
