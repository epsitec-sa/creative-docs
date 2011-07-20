//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	public enum DocumentOption
	{
		//	ATTENTION: Les noms des options sont sérialisés. Il ne faut donc pas les changer !

		None,

		Orientation,		// enum: Portrait, Landscape

		HeaderLogo,			// bool
		Specimen,			// bool

		FontSize,			// dimension: taille de la police

		LeftMargin,			// distance
		RightMargin,		// distance
		TopMargin,			// distance
		BottomMargin,		// distance

		LayoutFrame,		// enum: Frameless, WithLine, WithFrame
		LineNumber,			// enum: None, Group, Line, Full

		ArticleDelayed,		// bool
		ArticleId,			// bool

		ColumnsOrder,		// enum: QD, DQ

		IsrPosition,		// enum: WithInside, WithOutside, Without
		IsrType,			// enum: Isr, Is
		IsrFacsimile,		// bool

		Signing,			// bool

		RelationMail,		// bool
		RelationTelecom,	// bool
		RelationUri,		// bool
	}
}
