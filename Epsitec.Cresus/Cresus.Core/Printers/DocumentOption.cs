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

		Logo,
		Delayed,
		ArticleId,

		Frameless,
		WithLine,
		WithFrame,

		ColumnsOrderQD,
		ColumnsOrderDQ,

		ESR,
		ES,

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
