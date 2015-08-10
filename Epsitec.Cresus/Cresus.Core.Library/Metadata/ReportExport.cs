using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public sealed class ReportExport
	{
		public ReportExport(FormattedText text)
		{
			this.text = text;
		}

		public FormattedText Text
		{
			get
			{
				return this.text;
			}
		}

		public XElement Save()
		{
			var attributes = new List<XAttribute> ();

			var text = this.text.ToString ();
			attributes.Add (new XAttribute (Strings.Text, text));

			return new XElement (Strings.ReportExport, attributes);
		}

		public static ReportExport Restore(XElement xml)
		{
			if (xml.Name.LocalName != Strings.ReportExport)
			{
				throw new ArgumentException ("Invalid xml element name.");
			}

			var data = Xml.GetAttributeBag (xml);
			var text = new FormattedText (data[Strings.Text]);

			return new ReportExport (text);
		}

		#region Strings Class

		private static class Strings
		{
			public static readonly string		ReportExport = "report";
			public static readonly string		Text = "t";
		}

		#endregion

		private readonly FormattedText			text;
	}
}
