//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Server.Export
{
	[DesignerVisible]
	public enum ExportFormat
	{
		Unknown,
		Txt,
		Csv,
		Xml,
		Yaml,
		Json,
		Pdf,
	}
}