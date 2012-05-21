//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;

using Epsitec.BatchTool.SwissPostWebServices;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Tool.MailingColis
{
	class Program
	{
		static void Main(string[] args)
		{
			var format = new Epsitec.Common.IO.CsvFormat ()
			{
				Encoding = System.Text.Encoding.Default,
				FieldSeparator = ";",
				MultilineSeparator = "|",
				ColumnNames = new string[] { "Firme", "Titre", "Nom", "Prénom", "Adresse", "NPA", "Localité" }
			};

			var dataTable = CsvReader.ReadCsv ("..\\..\\colis.txt", format);
			var dataRows  = from row in dataTable.Rows.Cast<System.Data.DataRow> ().Skip (1)
							select new Address
							{
								Firme    = ((string) row[0]).Trim (),
								Titre    = ((string) row[1]).Trim (),
								Nom      = ((string) row[2]).Trim (),
								Prénom   = ((string) row[3]).Trim (),
								Adresse  = ((string) row[4]).Trim (),
								NPA      = ((string) row[5]).Trim (),
								Localité = ((string) row[6]).Trim (),
							};

			var addresses = dataRows.ToArray ();

			var withoutPrénom = addresses.Where (x => x.Prénom.Length == 0).ToArray ();
			var withoutNom    = addresses.Where (x => x.Nom.Length == 0).ToArray ();
			var withoutTitre  = addresses.Where (x => x.Titre.Length == 0).ToArray ();

			System.IO.File.WriteAllLines ("adresses.txt", (addresses.Where (x => x.Adresse.Split ('\n').First ().Length > 0 && !char.IsDigit (x.Adresse.Split ('\n').First ().Trim ().Split (' ').Last ()[0])).Select (x => x.Adresse.Split ('\n').First () + " > " + (x.Adresse.Split ('\n').Skip (1).FirstOrDefault () ?? ""))).ToArray ());
			System.IO.File.WriteAllLines ("cp.txt", (addresses.Where (x => x.Adresse.ContainsAnyWords ("cp", "c.p.", "case postale", "postfach")).Select (x => x.Adresse.Replace ("\n", " | ")).ToArray ()));
			System.IO.File.WriteAllLines ("full.csv", (addresses.Select (x => Engine.GetFullAddressLine (x))).ToArray ());

			Engine engine = new Engine ();
			
			int startWith = 0;
			int current = 0;

			foreach (var address in addresses)
			{
				if (current >= startWith)
				{
					System.Console.Out.WriteLine ("{0} : {1}, {2}", current, address.GetName1 (), address.Localité);
					
					engine.PrintLabel (address);

//					System.Threading.Thread.Sleep (4*1000);
				}

				current++;
			}


		}
	}
}
