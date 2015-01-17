//	Copyright � 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ArticleQuantityType
	{
		None				= 0,

		Ordered				= 10,	// command�
		Billed				= 20,	// factur�
		Delayed				= 30,	// retard�, confirm�e et/ou en suspens
		Expected			= 32,	// attendu, en suspens avec date non confirm�e
		Shipped				= 40,	// livr�
		ShippedPreviously	= 42,	// livr� pr�c�demment

		Information			= 100,	// information
	}
}
