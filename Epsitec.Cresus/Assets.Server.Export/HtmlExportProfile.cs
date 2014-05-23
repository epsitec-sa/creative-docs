//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour HtmlExport.
	/// </summary>
	public struct HtmlExportProfile
	{
		public HtmlExportProfile(string recordTag, bool isCompact)
		{
			this.RecordTag = recordTag;
			this.IsCompact = isCompact;
		}

		public static HtmlExportProfile Default = new HtmlExportProfile ("record", false);

		public readonly string					RecordTag;
		public readonly bool					IsCompact;
	}
}