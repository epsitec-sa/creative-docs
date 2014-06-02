//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour XmlExport.
	/// </summary>
	public class JsonExportProfile : AbstractExportProfile
	{
		public JsonExportProfile(string endOfLine, bool camelCase)
		{
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
		}

		public static JsonExportProfile Default = new JsonExportProfile ("\r\n", true);

		public readonly string					EndOfLine;
		public readonly bool					CamelCase;		public readonly bool					Compact;
	}
}