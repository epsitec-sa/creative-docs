//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	/// <summary>
	/// The <c>BookAccountDefinition</c> structure defines a book account (account number
	/// and caption). See class <see cref="CresusChartOfAccounts"/>.
	/// </summary>
	public struct BookAccountDefinition : System.IEquatable<BookAccountDefinition>, System.IComparable<BookAccountDefinition>
	{
		public BookAccountDefinition(string accountNumber, FormattedText caption)
		{
			this.accountNumber = accountNumber ?? "";
			this.caption       = caption;
		}

		internal BookAccountDefinition(Epsitec.CresusToolkit.CresusComptaCompte cresus)
		{
			this.accountNumber = cresus.Number ?? "";
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


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.Number, this.accountNumber),
				new XAttribute (Xml.Caption, this.caption.ToSimpleText ()));
		}

		public static BookAccountDefinition Restore(XElement xml)
		{
			string number  = (string) xml.Attribute (Xml.Number);
			string caption = (string) xml.Attribute (Xml.Caption);

			return new BookAccountDefinition (number, FormattedText.FromSimpleText (caption));
		}

		
		public override bool Equals(object obj)
		{
			if (obj is BookAccountDefinition)
			{
				return this.Equals ((BookAccountDefinition) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.accountNumber.GetHashCode () ^ this.caption.GetHashCode ();
		}

		
		#region IEquatable<BookAccountDefinition> Members

		public bool Equals(BookAccountDefinition other)
		{
			return this.accountNumber == other.accountNumber
				&& this.caption == other.caption;
		}

		#endregion

		#region IComparable<BookAccountDefinition> Members

		public int CompareTo(BookAccountDefinition other)
		{
			var result = string.CompareOrdinal (this.accountNumber, other.accountNumber);

			if (result == 0)
			{
				return this.caption.CompareTo (other.caption);
			}
			else
			{
				return result;
			}
		}

		#endregion

		
		private static class Xml
		{
			public const string Number		= "n";
			public const string Caption		= "c";
		}
	
		
		private readonly string				accountNumber;
		private readonly FormattedText		caption;
	}
}
