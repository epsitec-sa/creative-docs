//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public struct BookAccountDefinition
	{
		public BookAccountDefinition(string accountNumber, FormattedText caption)
		{
			this.accountNumber = accountNumber;
			this.caption = caption;
		}

		internal BookAccountDefinition(Epsitec.CresusToolkit.CresusComptaCompte cresus)
		{
			this.accountNumber = cresus.Number;
			this.caption       = FormattedText.FromSimpleText (cresus.Caption);
		}


		public string							AccountNumber
		{
			get
			{
				return this.accountNumber;
			}
		}

		public FormattedText					Caption
		{
			get
			{
				return this.caption;
			}
		}


		public XElement SerializeToXml(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (BookAccountDefinition.XmlNumber, this.accountNumber),
				new XAttribute (BookAccountDefinition.XmlCaption, this.caption.ToSimpleText ()));
		}

		public static BookAccountDefinition DeserializeFromXml(XElement xml)
		{
			string number  = (string) xml.Attribute (BookAccountDefinition.XmlNumber);
			string caption = (string) xml.Attribute (BookAccountDefinition.XmlCaption);

			return new BookAccountDefinition (number, FormattedText.FromSimpleText (caption));
		}

		private const string XmlNumber	= "n";
		private const string XmlCaption	= "c";
	
		private readonly string				accountNumber;
		private readonly FormattedText		caption;
	}
}
