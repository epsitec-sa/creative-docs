//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Aider.Data.Subscription
{
	internal sealed class ContactFileLine
	{
		public ContactFileLine(string id, string title, string lastname, string firstname, string corporateName,
							   string postBox, string addressComplement, string street, string houseNumber, string zipCode, string town,
							   ContactFileLineSource source)
		{
			this.Id                 = id ?? "";
			this.Title              = title ?? "";
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


		public static string GetHeader()
		{
			return "Id" + "\t"
				 + "Title" + "\t"
				 + "LastName" + "\t"
				 + "FirstName" + "\t"
				 + "CorporateName" + "\t"
				 + "PostBox" + "\t"
				 + "AddressComplement" + "\t"
				 + "Street" + "\t"
				 + "HouseNumber" + "\t"
				 + "ZipCode" + "\t"
				 + "Town" + "\t"
				 + ContactFileLine.LineEnding;
		}

		public string GetText()
		{
			return this.Id + "\t"
				 + this.Title + "\t"
				 + this.LastName + "\t"
				 + this.FirstName + "\t"
				 + this.CorporateName + "\t"
				 + this.PostBox + "\t"
				 + this.AddressComplement + "\t"
				 + this.Street + "\t"
				 + this.HouseNumber + "\t"
				 + this.ZipCode + "\t"
				 + this.Town + "\t"
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
		
		private static readonly string			LineEnding				   = "\r\n";
	}

	public enum ContactFileLineSource
	{
		Rch,
		Custom,
	}
}
