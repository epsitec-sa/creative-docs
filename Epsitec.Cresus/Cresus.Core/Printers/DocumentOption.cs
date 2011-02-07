//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum DocumentOption
	{
		None,

		HeaderLogo,
		ArticleDelayed,
		ArticleId,

		LayoutFrameless,
		LayoutWithLine,
		LayoutWithFrame,

		ColumnsOrderQD,
		ColumnsOrderDQ,

		InvoiceWithInsideESR,	// facture avec BV intégré
		InvoiceWithOutsideESR,	// facture avec BV séparé
		InvoiceWithoutESR,		// facture sans BV

		InvoiceWithESR,			// facture avec BVR
		InvoiceWithES,			// facture avec BV

		ESRFacsimile,
		Specimen,

		OrientationVertical,
		OrientationHorizontal,

		Signing,

		RelationMail,
		RelationTelecom,
		RelationUri,
	}
}
