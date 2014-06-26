﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour XmlExport.
	/// </summary>
	public class XmlExportProfile : AbstractExportProfile
	{
		public XmlExportProfile(string bodyTag, string recordTag, string indent, string endOfLine, bool camelCase, bool compact, Encoding encoding)
		{
			this.BodyTag   = bodyTag;
			this.RecordTag = recordTag;
			this.Indent    = indent;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Compact   = compact;
			this.Encoding  = encoding;
		}

		public static XmlExportProfile Default = new XmlExportProfile ("data", "record", "\t", "\r\n", true, false, Encoding.UTF8);

		public readonly string					BodyTag;
		public readonly string					RecordTag;
		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
		public readonly Encoding				Encoding;
	}
}