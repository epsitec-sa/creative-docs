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
	/// <summary>
	/// Cette classe s'occupe d'exporter les écritures pour Crésus Comptabilité
	/// dans les fichiers .ecc et .eca, sans utiliser la librairie FSC32.
	/// C'est un choix délibéré, car cette librairie est obsolète. Il faudra la
	/// réécrire dans un futur indéterminé...
	/// </summary>
	public class ExportEntries : System.IDisposable
	{
		public ExportEntries(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.eccLines = new List<EccLine> ();
			this.reports = new List<ExportEntriesReport> ();
		}

		public void Dispose()
		{
		}


		public string ExportFiles()
		{
			//	Génère toutes les écritures correspondant aux périodes des plans comptables connus.
			//	Retourne un texte résumant toutes les opérations effectuées.
			this.reports.Clear ();

			foreach (var range in this.accessor.Mandat.AccountsDateRanges)
			{
				var filename = this.accessor.Mandat.GetAccountsFilename (range);

				if (!string.IsNullOrEmpty (filename))
				{
					int entriesCount = this.ExportFile (range, filename);

					var report = new ExportEntriesReport (range, filename, entriesCount);
					this.reports.Add (report);
				}
			}

			return this.ReportsDescription;
		}


		private string ReportsDescription
		{
			get
			{
				if (this.reports.Any ())
				{
					var desc = string.Join ("<br/>", this.reports.Select (x => x.Description));
					return string.Format (Res.Strings.ExportEntries.Description.Many.ToString (), desc);
				}
				else
				{
					return Res.Strings.ExportEntries.Description.None.ToString ();
				}
			}
		}


		private int ExportFile(DateRange accountsRange, string accountsPath)
		{
			this.accountsRange = accountsRange;
			this.accountsPath  = accountsPath;

			//	On lit le fichier .ecc, sans conséquence si on n'y arrive pas.
			try
			{
				this.ReadEcc ();
			}
			catch (System.Exception ex)
			{
			}

			int uid = this.GetEccUid ();

			//	On lit le fichier .ecf, à la recherche du dernier idno utilisé.
			int idno;
			try
			{
				idno = this.GetLastIdno ();
			}
			catch
			{
				idno = 1;
			}

			const int nlot = 1;

			var data = this.GenerateEntriesData (accountsRange, nlot, uid, idno);
			System.IO.File.WriteAllText (this.EntriesPath, data, System.Text.Encoding.Default);

			this.CreateOrUpdateEccLine (uid);
			this.WriteEcc ();

			return this.entriesCount;
		}


		private int GetEccUid()
		{
			int uid = 1;

			foreach (var eccLine in this.eccLines)
			{
				if (eccLine.IsBody)
				{
					if (eccLine.Tag == ExportEntries.EccTag)
					{
						return eccLine.Uid.GetValueOrDefault (1);
					}
					else
					{
						uid = System.Math.Max (uid, eccLine.Uid.GetValueOrDefault () + 1);
					}
				}
			}

			return uid;
		}

		private int GetLastIdno()
		{
			//	Retourne le dernier idno (c'est-à-dire le numéro de la dernière écriture) utilisé pour
			//	l'ensemble des écritures générées par Assets, obtenu en relisant le fichier des écritures.
			int idno = 0;
			var lines = System.IO.File.ReadAllLines (this.EntriesPath, System.Text.Encoding.Default);

			foreach (var line in lines)
			{
				if (!string.IsNullOrEmpty (line) && !line.StartsWith ("#"))  // ligne d'écriture ?
				{
					var x = line.Split ('\t');
					if (x.Length >= 13)
					{
						int i = int.Parse (x[13]);
						idno = System.Math.Max (idno, i);
					}
				}
			}

			return idno + 1;
		}


		private string GenerateEntriesData(DateRange accountsRange, int nlot, int uid, int idno)
		{
			//	Retourne les données correspondant à l'ensemble des écritures générées par Assets.
			//	On met toujours un numéro nlot égal à uid.
			var builder = new System.Text.StringBuilder ();

			builder.Append (ExportEntries.entriesHeader);
			builder.Append ("\t");
			builder.Append (ExportEntries.type.ToUpper ());
			builder.Append ("\r\n");

			builder.Append ("#ECC\t1; ");
			builder.Append (nlot.ToStringIO ());  // nlot
			builder.Append ("; ");
			builder.Append (System.DateTime.Now.ToString ("dd.MM.yyyy HH:mm:ss"));
			builder.Append ("; ");
			builder.Append (uid.ToStringIO ());  // uid
			builder.Append ("\r\n");

			var entries = this.accessor.Mandat.GetData (BaseType.Entries);

			this.entriesCount = 0;
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

				if (date.HasValue && accountsRange.IsInside (date.Value) &&
					value.HasValue && value.Value != 0.0m)
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
					builder.Append ("\t");
					builder.Append ("0");  // cours
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("0");  // net
					builder.Append ("\t");
					builder.Append ("0");  // typtva
					builder.Append ("\t");
					builder.Append ("0");  // notva
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ((idno++).ToStringIO ());  // idno
					builder.Append ("\t");
					builder.Append (vatCode);
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("\t");
					builder.Append ("0");  // ?
					builder.Append ("\r\n");

					this.entriesCount++;
				}
			}

			return builder.ToString ();
		}


		private EccLine CreateOrUpdateEccLine(int uid)
		{
			//	Crée ou met à jour la ligne concernée dans le fichier .ecc.
			var eccLine = this.eccLines
				.Where (x => x.Tag == ExportEntries.EccTag && x.Filename == this.EntriesFilename)
				.FirstOrDefault ();

			if (eccLine == null)  // ligne inexistante ?
			{
				//	On crée une nouvelle ligne.
				eccLine = new EccLine (ExportEntries.EccTag, 1, System.DateTime.Now, this.EntriesFilename, uid);
				this.AddEccLine (eccLine);
			}
			else  // ligne trouvée ?
			{
				eccLine.Date = System.DateTime.Now;  // on met simplement à jour la date
				eccLine.Uid  = uid;
			}

			return eccLine;
		}

		private void AddEccLine(EccLine eccLine)
		{
			//	Ajoute une nouvelle ligne au fichier .ecc.
			if (!this.eccLines.Any ())  // fichier vide (inexistant) ?
			{
				//	On crée les lignes de début et de fin.
				this.eccLines.Add (new EccLine (ExportEntries.eccHeader));
				this.eccLines.Add (new EccLine (ExportEntries.eccFooter));
			}

			//	On insère la nouvelle ligne, toujours au début juste après la ligne #FSC.
			this.eccLines.Insert (1, eccLine);
		}

		private void ReadEcc()
		{
			//	Lit la totalité du fichier .ecc.
			this.eccLines.Clear ();

			var lines = System.IO.File.ReadAllLines (this.EccPath);
			foreach (var line in lines)
			{
				if (!string.IsNullOrEmpty (line))
				{
					var ecc = new EccLine (line);
					this.eccLines.Add (ecc);
				}
			}
		}

		private void WriteEcc()
		{
			//	Ecrit la totalité du fichier .ecc.
			System.IO.File.WriteAllLines (this.EccPath, this.eccLines.Select (x => x.Line), System.Text.Encoding.Default);
		}


		private string EccPath
		{
			//	Retourne le chemin du fichier de "pointeurs" vers les fichiers .ecf/.ecs/.eca,
			//	par exemple "C:\Documents Crésus\Exemples\Compta 2015.ecc".
			get
			{
				var dir = System.IO.Path.GetDirectoryName (this.accountsPath);
				var name = System.IO.Path.GetFileNameWithoutExtension (this.accountsPath);
				return System.IO.Path.Combine (dir, name + ".ecc");
			}
		}

		private string EntriesPath
		{
			//	Retourne le chemin du fichier contenant les écritures,
			//	par exemple "C:\Documents Crésus\Exemples\Mon Village (01-01-2015 ~ 31-12-2015).eca".
			get
			{
				var dir = System.IO.Path.GetDirectoryName (this.accountsPath);
				return System.IO.Path.Combine (dir, this.EntriesFilename);
			}
		}

		private string EntriesFilename
		{
			//	Retourne le nom du fichier contenant les écritures,
			//	par exemple "Mon Village (01-01-2015 ~ 31-12-2015).eca".
			get
			{
				string from = this.accountsRange.IncludeFrom           .ToString ("dd-MM-yyyy");
				string to   = this.accountsRange.ExcludeTo.AddDays (-1).ToString ("dd-MM-yyyy");

				return string.Format ("{0} ({1} ~ {2}).{3}", this.accessor.Mandat.Name, from, to, ExportEntries.type);
			}
		}

		private static string EccTag
		{
			//	Retourne le tag pour le fichier .ecc, normalement "#ECA".
			get
			{
				return string.Concat ("#", ExportEntries.type.ToUpper ());
			}
		}


		private const string					entriesHeader = "#FSC\t9.5.0";
		private const string					eccHeader = "#FSC\t9.3\tECC";
		private const string					eccFooter = "#END";
		//?private const string					type = "eca";  // nouveau, à voir avec MW
		private const string					type = "ecf";  // comme Crésus Facturation en attendant

		private readonly DataAccessor			accessor;
		private readonly List<EccLine>			eccLines;
		private readonly List<ExportEntriesReport> reports;

		private DateRange						accountsRange;
		private string							accountsPath;
		private int								entriesCount;
	}
}