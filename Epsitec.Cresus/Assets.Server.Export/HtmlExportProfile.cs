//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour HtmlExport.
	/// </summary>
	public class HtmlExportProfile : AbstractExportProfile
	{
		public HtmlExportProfile(string recordTag, string endOfLine, bool camelCase, bool compact)
		{
			this.RecordTag = recordTag;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Compact   = compact;
		}

		public static HtmlExportProfile Default = new HtmlExportProfile ("record", "\r\n", true, false);

		public readonly string					RecordTag;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
	}
}