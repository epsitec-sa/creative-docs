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
		public ReportExport(FormattedText text, string processor)
		{
			this.text = text;
			this.processor = processor;
		}

		public FormattedText Text
		{
			get
			{
				return this.text;
			}
		}

		public string Processor
		{
			get
			{
				return this.processor;
			}
		}

		public XElement Save()
		{
			var attributes = new List<XAttribute> ();

			var text = this.text.ToString ();
			var proc = this.processor;
			attributes.Add (new XAttribute (Strings.Text, text));
			attributes.Add (new XAttribute (Strings.Processor, proc));
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
			var proc = data[Strings.Processor];
			return new ReportExport (text, proc);
		}

		#region Strings Class

		private static class Strings
		{
			public static readonly string		ReportExport = "report";
			public static readonly string		Text         = "t";
			public static readonly string		Processor    = "p";
		}

		#endregion

		private readonly FormattedText			text;
		private readonly string			        processor;
	}
}
