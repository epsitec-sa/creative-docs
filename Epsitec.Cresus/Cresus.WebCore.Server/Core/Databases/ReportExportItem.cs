//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{
	/// <summary>
	/// The LabelExportItem class represents an entry that must appear in the export menu for a
	/// kind of label that can be exported.
	/// </summary>
	public sealed class ReportExportItem
	{
		public ReportExportItem(FormattedText text, string processor)
		{
			this.text = text;
			this.processor = processor;
		}


		public Dictionary<string, object> GetDataDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "text", this.text.ToString () },
				{ "processor", this.processor   }
			};
		}
		public readonly FormattedText			text;
		public readonly string                  processor;
	}
}
