//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public enum DataItemClass
	{
		None,

		Table,
		TableHeader1,
		TableHeader2,
		TableFooter1,
		TableFooter2,

		Group,
		GroupHeader1,
		GroupHeader2,
		GroupFooter1,
		GroupFooter2,

		Values,

		Aggregates,
	}
}
