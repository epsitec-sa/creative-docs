//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public class ExportEntries : System.IDisposable
	{
		public ExportEntries(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void Dispose()
		{
		}


		public int ExportFile(string filename)
		{
			var data = this.GetExportData (123);

			var dir = System.IO.Path.GetDirectoryName (filename);
			var name = this.accessor.Mandat.Name;
			filename = System.IO.Path.Combine (dir, name + ".ecf");

			System.IO.File.WriteAllText (filename, data, System.Text.Encoding.Unicode);

			return this.entryCount;
		}


		private string GetExportData(int uid)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append ("#FSC 7.0 ECF\r\n");

			builder.Append ("#ECC 1;");
			builder.Append (TypeConverters.IntToString (1));  // nlot
			builder.Append (";");
			builder.Append (TypeConverters.DateToString (Timestamp.Now.Date));
			builder.Append (";");
			builder.Append (uid.ToStringIO ());  // uid
			builder.Append ("\r\n");

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);

			this.entryCount = 0;
			int idno = 1;
			foreach (var entry in entries)
			{
				var date    = ObjectProperties.GetObjectPropertyDate    (entry, null, ObjectField.EntryDate);
				var debit   = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryDebitAccount);
				var credit  = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryCreditAccount);
				var stamp   = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryStamp);
				var title   = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryTitle);
				var value   = ObjectProperties.GetObjectPropertyDecimal (entry, null, ObjectField.EntryAmount);
				var vatCode = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryVatCode);

				if (vatCode.Length == 1)  // pas de code TVA (par exemple "-") ?
				{
					vatCode = null;
				}

				if (value.HasValue && value.Value != 0.0m)
				{
					builder.Append (TypeConverters.DateToString (date));
					builder.Append ("\t");
					builder.Append (debit);
					builder.Append ("\t");
					builder.Append (credit);
					builder.Append ("\t");
					builder.Append (stamp);
					builder.Append ("\t");
					builder.Append (title);
					builder.Append ("\t");
					builder.Append (value.Value.ToStringIO ());
					builder.Append ("\t");
					builder.Append ("0");  // montant_me
					builder.Append ("\t");
					builder.Append ("0");  // cours
					builder.Append ("\t");
					builder.Append ("0");  // nmult
					builder.Append ("\t");
					builder.Append ("0");  // net
					builder.Append ("\t");
					builder.Append ("0");  // typtva
					builder.Append ("\t");
					builder.Append ("0");  // notva
					builder.Append ("\t");
					builder.Append ("0");  // unused
					builder.Append ("\t");
					builder.Append ((idno++).ToStringIO ());  // idno
					builder.Append ("\t");
					builder.Append (vatCode);
					builder.Append ("\r\n");

					this.entryCount++;
				}
			}

			return builder.ToString ();
		}
	
	
	
		private DataAccessor					accessor;
		private int								entryCount;
	}
}