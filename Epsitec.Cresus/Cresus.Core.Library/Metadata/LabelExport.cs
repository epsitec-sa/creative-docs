using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public sealed class LabelExport
	{
		public LabelExport(int textFactoryId, FormattedText text)
		{
			this.textFactoryId = textFactoryId;
			this.text = text;
		}

		public int TextFactoryId
		{
			get
			{
				return this.textFactoryId;
			}
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

			var textFactoryId = InvariantConverter.ToString (this.textFactoryId);
			attributes.Add (new XAttribute (Strings.TextFactoryId, textFactoryId));
			
			var text = this.text.ToString ();
			attributes.Add (new XAttribute (Strings.Text, text));

			return new XElement (Strings.LabelExport, attributes);
		}

		public static LabelExport Restore(XElement xml)
		{
			if (xml.Name.LocalName != Strings.LabelExport)
			{
				throw new ArgumentException ("Invalid xml element name.");
			}

			var data = Xml.GetAttributeBag (xml);

			var textFactoryId = InvariantConverter.ParseInt (data[Strings.TextFactoryId]);
			var text = new FormattedText (data[Strings.Text]);

			return new LabelExport (textFactoryId, text);
		}

		#region Strings Class

		private static class Strings
		{
			public static readonly string		LabelExport = "l";
			public static readonly string		TextFactoryId = "f";
			public static readonly string		Text = "t";
		}

		#endregion

		private readonly int					textFactoryId;
		private readonly FormattedText			text;
	}
}
