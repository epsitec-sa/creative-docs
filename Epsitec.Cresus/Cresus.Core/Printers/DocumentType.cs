//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum DocumentType
	{
		None,

		Offer,					// offre
		Order,					// commande
		OrderAcknowledge,		// confirmation de commande
		ProductionOrder,		// ordre de production
		BL,						// bulletin de livraison
		Invoice,				// facture

		Summary,
		Debug1,
		Debug2,
	}
}
