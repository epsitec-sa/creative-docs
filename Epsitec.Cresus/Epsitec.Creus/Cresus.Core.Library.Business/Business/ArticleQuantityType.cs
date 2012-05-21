//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ArticleQuantityType
	{
		None				= 0,

		Ordered				= 10,	// commandé
		Billed				= 20,	// facturé
		Delayed				= 30,	// retardé, confirmée et/ou en suspens
		Expected			= 32,	// attendu, en suspens avec date non confirmée
		Shipped				= 40,	// livré
		ShippedPreviously	= 42,	// livré précédemment

		Information			= 100,	// information
	}
}
