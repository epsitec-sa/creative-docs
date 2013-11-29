//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Aider.Data.Subscription
{
	internal sealed class ContactFileLine
	{
		public ContactFileLine(string id, PersonMrMrs? mrMrs, string lastname, string firstname, string corporateName,
							   string postBox, string addressComplement, string street, string houseNumber, string zipCode, string town,
							   ContactFileLineSource source)
		{
			this.Id                 = id ?? "";
			this.Title              = mrMrs == null ? "" : ContactFileLine.GetTitle (mrMrs.Value);
			this.LastName           = lastname ?? "";
			this.FirstName          = firstname ?? "";
			this.CorporateName      = corporateName ?? "";
			this.PostBox            = postBox ?? "";
			this.AddressComplement  = addressComplement ?? "";
			this.Street             = street ?? "";
			this.HouseNumber        = houseNumber ?? "";
			this.ZipCode            = zipCode ?? "";
			this.Town               = town ?? "";
			this.Source             = source;
		}

		private static string GetTitle(PersonMrMrs personMrMrs)
		{
			switch (personMrMrs)
			{
				case PersonMrMrs.Madame:
					return "Madame";
				case PersonMrMrs.Mademoiselle:
					return "Mademoiselle";
				case PersonMrMrs.Monsieur:
					return "Monsieur";
				default:
					return "";
			}
		}


		public static string GetHeader()
		{
			return "Id" + ContactFileLine.Separator
				 + "Title" + ContactFileLine.Separator
				 + "LastName" + ContactFileLine.Separator
				 + "FirstName" + ContactFileLine.Separator
				 + "CorporateName" + ContactFileLine.Separator
				 + "PostBox" + ContactFileLine.Separator
				 + "AddressComplement" + ContactFileLine.Separator
				 + "Street" + ContactFileLine.Separator
				 + "HouseNumber" + ContactFileLine.Separator
				 + "ZipCode" + ContactFileLine.Separator
				 + "Town" + ContactFileLine.Separator
				 + ContactFileLine.LineEnding;
		}

		public string GetText()
		{
			return this.Id + ContactFileLine.Separator
				 + this.Title + ContactFileLine.Separator
				 + this.LastName + ContactFileLine.Separator
				 + this.FirstName + ContactFileLine.Separator
				 + this.CorporateName + ContactFileLine.Separator
				 + this.PostBox + ContactFileLine.Separator
				 + this.AddressComplement + ContactFileLine.Separator
				 + this.Street + ContactFileLine.Separator
				 + this.HouseNumber + ContactFileLine.Separator
				 + this.ZipCode + ContactFileLine.Separator
				 + this.Town + ContactFileLine.Separator
				 + ContactFileLine.LineEnding;
		}

		
		public override string ToString()
		{
			return this.GetText ();
		}

		public static void Write(IEnumerable<ContactFileLine> lines, FileInfo file)
		{
			var encoding = ContactFileLine.DefaultEncoding;

			using (var stream = file.Open (FileMode.Create, FileAccess.Write))
			using (var streamWriter = new StreamWriter (stream, encoding))
			{
				streamWriter.Write (ContactFileLine.GetHeader ());

				foreach (var line in lines)
				{
					// The text contains the line ending.
					var text = line.GetText ();

					streamWriter.Write (text);
				}
			}
		}

		public static readonly Encoding			DefaultEncoding = Encoding.GetEncoding ("ISO-8859-1");

		public readonly string					Id;
		public readonly string					Title;
		public readonly string					LastName;
		public readonly string					FirstName;
		public readonly string					CorporateName;
		public readonly string					PostBox;
		public readonly string					AddressComplement;
		public readonly string					Street;
		public readonly string					HouseNumber;
		public readonly string					ZipCode;
		public readonly string					Town;
		public readonly ContactFileLineSource	Source;
		
		private static readonly string			LineEnding		= "\r\n";
		private static readonly string			Separator		= ";";
	}
}