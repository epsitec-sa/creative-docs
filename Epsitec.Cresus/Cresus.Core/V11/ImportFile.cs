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
			this.records = new List<V11Record> ();
		}


		/// <summary>
		/// Choix interactif d'un fichier *.v11 puis importation des données.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="noClient"></param>
		/// <returns></returns>
		public V11Message Import(CoreApplication application, string noClient)
		{
			string filename = this.OpenFileDialog (application);

			if (string.IsNullOrEmpty (filename))
			{
				return V11Message.Aborted;
			}

			V11Message message = this.ReadFile (application, filename, noClient);

			if (message.IsError)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message.Description.ToString ()).OpenDialog ();
			}

			return message;
		}

		/// <summary>
		/// Retourne la liste des données importées.
		/// </summary>
		public List<V11Record> Records
		{
			get
			{
				return this.records;
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

		private V11Message ReadFile(CoreApplication application, string filename, string noClient)
		{
			if (string.IsNullOrWhiteSpace (noClient))
			{
				noClient = "10694443";  // TODO: provisoire...
			}

			noClient = noClient.TrimStart ('0');

			string[] records;

			try
			{
				records = System.IO.File.ReadAllLines (filename);
			}
			catch (System.Exception e)
			{
				FormattedText description = TextFormatter.FormatText ("Impossible d'ouvrir le fichier", filename, "<br/><br/>", e.Message);
				return new V11Message (V11Error.FileNotFound, null, description);
			}

			this.records.Clear ();
			V11Record.TypeEnum type = V11Record.TypeEnum.Unknown;
			int lineRank = -1;

			foreach (var line in records)
			{
				lineRank++;

				if (line.Length < 100)
				{
					continue;
				}

				if (type == V11Record.TypeEnum.Unknown)  // type inconnu ?
				{
					string s = line.Substring (3, 9).TrimStart ('0');
					if (s == noClient)
					{
						type = V11Record.TypeEnum.Type3;
					}
					else
					{
						s = line.Substring (6, 9).TrimStart ('0');
						if (s == noClient)
						{
							type = V11Record.TypeEnum.Type4;
						}
						else
						{
							FormattedText description = TextFormatter.FormatText ("Aucune donnée pour le client", noClient);
							return new V11Message (V11Error.EmptyData, null, description);
						}
					}

					var record = new V11Record ();
					record.Type = type;

					if (type == V11Record.TypeEnum.Type3)
					{
						record.CodeTransaction  = V11Record.CodeTransactionEnum.Normal;
						record.GenreTransaction = ImportFile.StringToGenreTransaction (line.Substring (0, 1));
						record.Origine          = V11Record.OrigineEnum.OfficePoste;
						record.GenreRemise      = V11Record.GenreRemiseEnum.Original;
						record.NoReference      = line.Substring (12, 27);
						record.MonnaieMontant   = "CHF";
						record.Montant          = ImportFile.StringToPrice (line.Substring (39, 10));
						record.RefDepot         = line.Substring (49, 10);
						record.DateDepot        = ImportFile.StringToDate (line.Substring (59, 6));
						record.DateTraitement   = ImportFile.StringToDate (line.Substring (65, 6));
						record.DateCredit       = ImportFile.StringToDate (line.Substring (71, 6));
						record.NoMicrofilm      = line.Substring (77, 9).TrimStart ('0');
						record.CodeRejet        = ImportFile.StringToCodeRejet (line.Substring (86, 1));
						record.MonnaieTaxes     = "CHF";
						record.Taxes            = ImportFile.StringToPrice (line.Substring (96, 4));
					}

					if (type == V11Record.TypeEnum.Type4)
					{
						ImportFile.StringToTransaction (record, line.Substring (0, 2));
						record.GenreTransaction = ImportFile.StringToGenreTransaction (line.Substring (2, 1));
						record.Origine          = ImportFile.StringToOrigine (line.Substring (3, 2));
						record.GenreRemise      = ImportFile.StringToGenreRemise (line.Substring (5, 1));
						record.NoReference      = line.Substring (15, 27);
						record.MonnaieMontant   = line.Substring (42, 3);
						record.Montant          = ImportFile.StringToPrice (line.Substring (45, 10));
						record.RefDepot         = "";
						record.DateDepot        = ImportFile.StringToDate (line.Substring (92, 8));
						record.DateTraitement   = ImportFile.StringToDate (line.Substring (100, 8));
						record.DateCredit       = ImportFile.StringToDate (line.Substring (108, 8));
						record.NoMicrofilm      = "";
						record.CodeRejet        = ImportFile.StringToCodeRejet (line.Substring (116, 1));
						record.MonnaieTaxes     = line.Substring (117, 3);
						record.Taxes            = ImportFile.StringToPrice (line.Substring (120, 6));
					}

					if (!record.IsValid)
					{
						FormattedText description = TextFormatter.FormatText ("Erreur à la ligne", (lineRank+1).ToString ());
						return new V11Message (V11Error.InvalidFormat, lineRank, description);
					}

					this.records.Add (record);
				}
			}

			if (this.records.Count == 0)
			{
				FormattedText description = TextFormatter.FormatText ("Aucune donnée pour le client", noClient);
				return new V11Message (V11Error.EmptyData, null, description);
			}

			return V11Message.OK;
		}


		private static void StringToTransaction(V11Record record, string text)
		{
			switch (text)
			{
				case "01":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "02":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Remboursement;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;

				case "03":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "CHF";
					break;


				case "11":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;

				case "13":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "CHF";
					break;


				case "21":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;

				case "22":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Remboursement;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;

				case "23":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVR;
					record.MonnaieTransaction = "EUR";
					break;


				case "31":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Normal;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "EUR";
					break;

				case "33":
					record.CodeTransaction    = V11Record.CodeTransactionEnum.PropreCompte;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.BVRPlus;
					record.MonnaieTransaction = "EUR";
					break;


				default:
					record.CodeTransaction    = V11Record.CodeTransactionEnum.Unknown;
					record.BVRTransaction     = V11Record.BVRTransactionEnum.Unknown;
					record.MonnaieTransaction = null;
					break;
			}
		}

		private static V11Record.GenreTransactionEnum StringToGenreTransaction(string text)
		{
			switch (text)
			{
				case "1":
					return V11Record.GenreTransactionEnum.Credit;

				case "2":
					return V11Record.GenreTransactionEnum.ContrePrestation;

				case "3":
					return V11Record.GenreTransactionEnum.Correction;

				default:
					return V11Record.GenreTransactionEnum.Unknown;

			}
		}

		private static V11Record.OrigineEnum StringToOrigine(string text)
		{
			switch (text)
			{
				case "01":
					return V11Record.OrigineEnum.OfficePoste;

				case "02":
					return V11Record.OrigineEnum.OPA;

				case "03":
					return V11Record.OrigineEnum.yellownet;

				case "04":
					return V11Record.OrigineEnum.EuroSIC;

				default:
					return V11Record.OrigineEnum.Unknown;

			}
		}

		private static V11Record.GenreRemiseEnum StringToGenreRemise(string text)
		{
			switch (text)
			{
				case "1":
					return V11Record.GenreRemiseEnum.Original;

				case "2":
					return V11Record.GenreRemiseEnum.Reconstruction;

				case "3":
					return V11Record.GenreRemiseEnum.Test;

				default:
					return V11Record.GenreRemiseEnum.Unknown;

			}
		}

		private static V11Record.CodeRejetEnum StringToCodeRejet(string text)
		{
			switch (text)
			{
				case "0":
					return V11Record.CodeRejetEnum.Aucun;

				case "1":
					return V11Record.CodeRejetEnum.Rejet;

				case "5":
					return V11Record.CodeRejetEnum.RejetMasse;

				default:
					return V11Record.CodeRejetEnum.Unknown;

			}
		}

		private static int? StringToInt(string text)
		{
			text = text.TrimStart ('0');

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
			int year, month, day;

			if (text.Length == 6)
			{
				if (!int.TryParse (text.Substring (0, 2), out year))
				{
					return null;
				}

				if (!int.TryParse (text.Substring (2, 2), out month))
				{
					return null;
				}

				if (!int.TryParse (text.Substring (4, 2), out day))
				{
					return null;
				}

				return new Date (2000+year, month, day);
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

				return new Date (year, month, day);
			}

			return null;
		}


		private readonly List<V11Record>		records;
		private string							initialDirectory;
	}
}
