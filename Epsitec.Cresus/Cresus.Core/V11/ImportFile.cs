//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class ImportFile
	{
		public ImportFile()
		{
			this.records = new List<V11AbstractLine> ();
			this.errors  = new List<V11Message> ();
		}


		/// <summary>
		/// Choix interactif d'un fichier *.v11 puis importation des données.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="noClient"></param>
		/// <returns></returns>
		public V11Message Import(CoreApplication application)
		{
			string filename = this.OpenFileDialog (application);

			if (string.IsNullOrEmpty (filename))
			{
				return V11Message.Aborted;
			}

			this.ReadFile (application, filename);

			if (this.errors.Count > 0)
			{
				FormattedText description = V11Message.Descriptions (this.errors);
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, description.ToString ()).OpenDialog ();

				return V11Message.GenericError;
			}
			else
			{
				FormattedText description = TextFormatter.FormatText ("Importation réussie de", this.records.Count.ToString(), "lignes");
				MessageDialog.CreateOk ("Terminé", DialogIcon.Question, description.ToString ()).OpenDialog ();

				return V11Message.OK;
			}
		}

		/// <summary>
		/// Retourne la liste des données importées.
		/// </summary>
		public List<V11AbstractLine> Records
		{
			get
			{
				return this.records;
			}
		}

		public List<V11Message> Errors
		{
			get
			{
				return this.errors;
			}
		}


		private string OpenFileDialog(CoreApplication application)
		{
			var dialog = new FileOpenDialog ();

			dialog.Title = "Importation d'un fichier de paiements de type \"V11\"";
			dialog.InitialDirectory = this.initialDirectory;
			//?dialog.FileName = "";

			dialog.Filters.Add ("v11", "Fichier de paiements", "*.v11;*.bvr;*.esr");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.Owner = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			this.initialDirectory = dialog.InitialDirectory;

			return dialog.FileName;
		}

		private void ReadFile(CoreApplication application, string filename)
		{
			this.records.Clear ();
			this.errors.Clear ();

			string[] lines;

			try
			{
				lines = System.IO.File.ReadAllLines (filename);
			}
			catch (System.Exception e)
			{
				FormattedText description = TextFormatter.FormatText ("Impossible d'ouvrir le fichier", filename, "<br/><br/>", e.Message);
				this.errors.Add (new V11Message (V11Error.FileNotFound, null, null, description));
				return;
			}

			V11AbstractLine.TypeEnum type = V11AbstractLine.TypeEnum.Unknown;
			int lineRank = -1;

			foreach (var l in lines)
			{
				string line = l.TrimEnd ();  // supprime les espaces en fin de ligne
				lineRank++;  // 0..n

				if (string.IsNullOrWhiteSpace (line))
				{
					continue;  // une ligne vide ne génère pas d'erreur
				}

				if (type == V11AbstractLine.TypeEnum.Unknown)  // type inconnu ?
				{
					if (line.Length >= 126)
					{
						type = V11AbstractLine.TypeEnum.Type4;
					}
					else if (line.Length >= 100)
					{
						type = V11AbstractLine.TypeEnum.Type3;
					}
					else
					{
						FormattedText description = TextFormatter.FormatText ("Erreur à la ligne", (lineRank+1).ToString ());
						this.errors.Add (new V11Message (V11Error.InvalidFormat, lineRank, line, description));
						continue;
					}
				}

				V11AbstractLine abstractRecord = null;

				if (type == V11AbstractLine.TypeEnum.Type3)
				{
					string genre = line.Substring (0, 3);

					if (genre == "999" || genre == "995")
					{
						var record = new V11TotalLine (type);

						record.GenreTransaction  = (genre == "996") ? V11AbstractLine.GenreTransactionEnum.ContrePrestation : V11AbstractLine.GenreTransactionEnum.Credit;
						record.NoClient          = line.Substring (3, 9).TrimStart ('0');
						record.GenreRemise       = V11AbstractLine.GenreRemiseEnum.Original;
						record.CleTri            = line.Substring (12, 27);
						record.MonnaieMontant    = "CHF";
						record.Montant           = ImportFile.StringToPrice (line.Substring (39, 12));
						record.NbTransactions    = ImportFile.StringToInt (line.Substring (51, 12));
						record.DateEtablissement = ImportFile.StringToDate (line.Substring (63, 6));
						record.MonnaieTaxes      = "CHF";
						record.Taxes             = ImportFile.StringToPrice (line.Substring (69, 9));

						abstractRecord = record;
					}
					else
					{
						var record = new V11RecordLine (type);

						ImportFile.StringToTransaction3 (record, line.Substring (0, 3));
						record.Origine           = V11RecordLine.OrigineEnum.OfficePoste;
						record.NoClient          = line.Substring (3, 9).TrimStart ('0');
						record.GenreRemise       = V11AbstractLine.GenreRemiseEnum.Original;
						record.NoReference       = line.Substring (12, 27);
						record.MonnaieMontant    = "CHF";
						record.Montant           = ImportFile.StringToPrice (line.Substring (39, 10));
						record.RefDepot          = line.Substring (49, 10);
						record.DateDepot         = ImportFile.StringToDate (line.Substring (59, 6));
						record.DateTraitement    = ImportFile.StringToDate (line.Substring (65, 6));
						record.DateCredit        = ImportFile.StringToDate (line.Substring (71, 6));
						record.NoMicrofilm       = line.Substring (77, 9).TrimStart ('0');
						record.CodeRejet         = ImportFile.StringToCodeRejet (line.Substring (86, 1));
						record.MonnaieTaxes      = "CHF";
						record.Taxes             = ImportFile.StringToPrice (line.Substring (96, 4));

						abstractRecord = record;
					}
				}

				if (type == V11AbstractLine.TypeEnum.Type4)
				{
					string genre = line.Substring (0, 2);

					if (genre == "99" || genre == "98")
					{
						var record = new V11TotalLine (type);

						record.GenreTransaction  = ImportFile.StringToGenreTransaction (line.Substring (2, 1));
						record.NoClient          = line.Substring (6, 9).TrimStart ('0');
						record.GenreRemise       = ImportFile.StringToGenreRemise (line.Substring (5, 1));
						record.CleTri            = line.Substring (15, 27);
						record.MonnaieMontant    = line.Substring (42, 3);
						record.Montant           = ImportFile.StringToPrice (line.Substring (45, 12));
						record.NbTransactions    = ImportFile.StringToInt (line.Substring (57, 12));
						record.DateEtablissement = ImportFile.StringToDate (line.Substring (69, 8));
						record.MonnaieTaxes      = line.Substring (77, 3);
						record.Taxes             = ImportFile.StringToPrice (line.Substring (80, 11));

						abstractRecord = record;
					}
					else
					{
						var record = new V11RecordLine (type);

						ImportFile.StringToTransaction4 (record, line.Substring (0, 2));
						record.GenreTransaction  = ImportFile.StringToGenreTransaction (line.Substring (2, 1));
						record.Origine           = ImportFile.StringToOrigine (line.Substring (3, 2));
						record.NoClient          = line.Substring (6, 9).TrimStart ('0');
						record.GenreRemise       = ImportFile.StringToGenreRemise (line.Substring (5, 1));
						record.NoReference       = line.Substring (15, 27);
						record.MonnaieMontant    = line.Substring (42, 3);
						record.Montant           = ImportFile.StringToPrice (line.Substring (45, 12));
						record.RefDepot          = "";
						record.DateDepot         = ImportFile.StringToDate (line.Substring (92, 8));
						record.DateTraitement    = ImportFile.StringToDate (line.Substring (100, 8));
						record.DateCredit        = ImportFile.StringToDate (line.Substring (108, 8));
						record.NoMicrofilm       = "";
						record.CodeRejet         = ImportFile.StringToCodeRejet (line.Substring (116, 1));
						record.MonnaieTaxes      = line.Substring (117, 3);
						record.Taxes             = ImportFile.StringToPrice (line.Substring (120, 6));

						abstractRecord = record;
					}
				}

				if (abstractRecord != null && abstractRecord.IsValid)
				{
					this.records.Add (abstractRecord);
				}
				else
				{
					FormattedText description = TextFormatter.FormatText ("Erreur à la ligne", (lineRank+1).ToString ());
					this.errors.Add (new V11Message (V11Error.InvalidFormat, lineRank, line, description));
				}
			}
		}


		private static void StringToTransaction3(V11RecordLine record, string text)
		{
			switch (text)
			{
				case "002":
				case "012":
				case "022":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Credit;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "005":
				case "015":
				case "025":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.ContrePrestation;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "008":
				case "018":
				case "028":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Correction;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;


				case "032":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Credit;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "035":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.ContrePrestation;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "038":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Correction;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;


				case "102":
				case "112":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Credit;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "105":
				case "115":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.ContrePrestation;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "108":
				case "118":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Correction;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;


				case "132":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Credit;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "135":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.ContrePrestation;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "138":
					record.GenreTransaction   = V11AbstractLine.GenreTransactionEnum.Correction;
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;


				default:
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Unknown;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.Unknown;
					record.MonnaieTransaction = null;
					break;
			}
		}

		private static void StringToTransaction4(V11RecordLine record, string text)
		{
			switch (text)
			{
				case "01":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "02":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Remboursement;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "03":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;


				case "11":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "13":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;


				case "21":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;

				case "22":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Remboursement;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;

				case "23":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;


				case "31":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "EUR";
					break;

				case "33":
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "EUR";
					break;


				default:
					record.CodeTransaction    = V11RecordLine.CodeTransactionEnum.Unknown;
					record.BVRTransaction     = V11RecordLine.BVRTransactionEnum.Unknown;
					record.MonnaieTransaction = null;
					break;
			}
		}

		private static V11AbstractLine.GenreTransactionEnum StringToGenreTransaction(string text)
		{
			switch (text)
			{
				case "1":
					return V11AbstractLine.GenreTransactionEnum.Credit;

				case "2":
					return V11AbstractLine.GenreTransactionEnum.ContrePrestation;

				case "3":
					return V11AbstractLine.GenreTransactionEnum.Correction;

				default:
					return V11AbstractLine.GenreTransactionEnum.Unknown;

			}
		}

		private static V11RecordLine.OrigineEnum StringToOrigine(string text)
		{
			switch (text)
			{
				case "01":
					return V11RecordLine.OrigineEnum.OfficePoste;

				case "02":
					return V11RecordLine.OrigineEnum.OPA;

				case "03":
					return V11RecordLine.OrigineEnum.yellownet;

				case "04":
					return V11RecordLine.OrigineEnum.EuroSIC;

				default:
					return V11RecordLine.OrigineEnum.Unknown;

			}
		}

		private static V11AbstractLine.GenreRemiseEnum StringToGenreRemise(string text)
		{
			switch (text)
			{
				case "1":
					return V11AbstractLine.GenreRemiseEnum.Original;

				case "2":
					return V11AbstractLine.GenreRemiseEnum.Reconstruction;

				case "3":
					return V11AbstractLine.GenreRemiseEnum.Test;

				default:
					return V11AbstractLine.GenreRemiseEnum.Unknown;

			}
		}

		private static V11RecordLine.CodeRejetEnum StringToCodeRejet(string text)
		{
			switch (text)
			{
				case "0":
					return V11RecordLine.CodeRejetEnum.Aucun;

				case "1":
					return V11RecordLine.CodeRejetEnum.Rejet;

				case "5":
					return V11RecordLine.CodeRejetEnum.RejetMasse;

				default:
					return V11RecordLine.CodeRejetEnum.Unknown;

			}
		}

		private static int? StringToInt(string text)
		{
			text = text.TrimStart ('0');

			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}

			int value;
			if (int.TryParse (text, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		private static decimal? StringToPrice(string text)
		{
			//	Par exemple "0000029800" pour 298.00 francs.
			text = text.TrimStart ('0');

			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}

			decimal value;
			if (decimal.TryParse (text, out value))
			{
				return value/100;
			}
			else
			{
				return null;
			}
		}

		private static Date? StringToDate(string text)
		{
			//	Par exemple "080109" pour 09.01.2008
			//	Par exemple "20080109" pour 09.01.2008
			int year  = 0;
			int month = 0;
			int day   = 0;

			if (text.Length == 6)
			{
				if (!int.TryParse (text.Substring (0, 2), out year))
				{
					return null;
				}

				year += 2000;

				if (!int.TryParse (text.Substring (2, 2), out month))
				{
					return null;
				}

				if (!int.TryParse (text.Substring (4, 2), out day))
				{
					return null;
				}
			}

			if (text.Length == 8)
			{
				if (!int.TryParse (text.Substring (0, 4), out year))
				{
					return null;
				}

				if (!int.TryParse (text.Substring (4, 2), out month))
				{
					return null;
				}

				if (!int.TryParse (text.Substring (6, 2), out day))
				{
					return null;
				}
			}

			if (year >= 2000 && year <= 2099 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
			{
				try
				{
					return new Date (year, month, day);
				}
				catch
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}


		private readonly List<V11AbstractLine>		records;
		private readonly List<V11Message>			errors;
		private string								initialDirectory;
	}
}
