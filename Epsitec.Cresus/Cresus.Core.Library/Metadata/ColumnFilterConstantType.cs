//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Core.Metadata
{
	public enum ColumnFilterConstantType
	{
		Undefined,

		Integer,
		Decimal,
		Date,
		Time,
		DateTime,
		String,
		Enumeration,
		EntityKey,
	}
}
