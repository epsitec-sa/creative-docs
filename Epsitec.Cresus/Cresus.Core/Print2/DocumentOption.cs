//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2
{
	public enum DocumentOption
	{
		None,


		Orientation,		// Postrait, Landscape

		HeaderLogo,
		Specimen,

		Margins,
		LeftMargin,
		RightMargin,
		TopMargin,
		BottomMargin,

		LayoutFrame,		// Frameless, WithLine, WithFrame
	
		ArticleDelayed,
		ArticleId,

		ColumnsOrder,		// QD, DQ
		
		EsrPosition,		// WithInside, WithOutside, Without
		EsrType,			// Esr, Es
		EsrFacsimile,

		Signing,

		RelationMail,
		RelationTelecom,
		RelationUri,
	}
}
