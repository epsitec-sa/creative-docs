//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ArticleQuantityType
	{
		None				= 0,

		Ordered				= 10,
		Billed				= 20,
		Delayed				= 30,
		Shipped				= 40,
		ShippedPreviously	= 42,

		Information			= 100,
	}
}
