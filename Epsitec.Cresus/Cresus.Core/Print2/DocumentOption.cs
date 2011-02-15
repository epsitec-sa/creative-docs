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

		Orientation,		// enum: Portrait, Landscape

		HeaderLogo,			// bool
		Specimen,			// bool

		Margins,			// distance
		LeftMargin,			// distance
		RightMargin,		// distance
		TopMargin,			// distance
		BottomMargin,		// distance

		LayoutFrame,		// enum: Frameless, WithLine, WithFrame

		ArticleDelayed,		// bool
		ArticleId,			// bool

		ColumnsOrder,		// enum: QD, DQ

		EsrPosition,		// enum: WithInside, WithOutside, Without
		EsrType,			// enum: Esr, Es
		EsrFacsimile,		// bool

		Signing,			// bool

		RelationMail,		// bool
		RelationTelecom,	// bool
		RelationUri,		// bool
	}
}
