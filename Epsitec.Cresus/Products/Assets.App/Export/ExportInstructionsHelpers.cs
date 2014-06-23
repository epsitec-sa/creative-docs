//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class ExportInstructionsHelpers
	{
		public static string ForceExt(string filename, string ext)
		{
			//	Retourne un nom de fichier complet (avec chemin d'accès) dont on a forcé l'extension.
			if (string.IsNullOrEmpty (filename))
			{
				return null;
			}
			else
			{
				return System.IO.Path.Combine (
					System.IO.Path.GetDirectoryName (filename),
					System.IO.Path.GetFileNameWithoutExtension (filename) + ext);
			}
		}


		public static ExportFormat GetFormat(int rank)
		{
			//	Retourne un format d'après son rang.
			var list = ExportInstructionsHelpers.Formats.ToArray ();

			if (rank >= 0 && rank < list.Length)
			{
				return list[rank];
			}
			else
			{
				return ExportFormat.Unknown;
			}
		}

		public static int GetRank(ExportFormat format)
		{
			//	Retourne le rang d'un format, ou -1.
			var list = ExportInstructionsHelpers.Formats.ToList ();
			return list.IndexOf (format);
		}

		public static string MultiLabels
		{
			//	Retourne le texte permettant de créer des boutons radios.
			get
			{
				return string.Join ("<br/>", ExportInstructionsHelpers.Formats.Select (x => ExportInstructionsHelpers.GetFormatName (x)));
			}
		}

		public static string GetFormatName(ExportFormat format)
		{
			//	Retourne le nom en clair d'un format.
			switch (format)
			{
				case ExportFormat.Txt:
					return "TXT — Fichier texte tabulé";

				case ExportFormat.Csv:
					return "CSV — Fichier texte pour tableur";

				case ExportFormat.Xml:
					return "XML — Fichier texte \"Extensible Markup Language\"";

				case ExportFormat.Yaml:
					return "YAML — Fichier texte \"YAML Ain't Markup Language\"";

				case ExportFormat.Json:
					return "JSON — Fichier texte \"JavaScript Object Notation\"";

				case ExportFormat.Pdf:
					return "PDF — Document mis en pages";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid format", format));
			}
		}

		public static string GetFormatExt(ExportFormat format)
		{
			//	Retourne l'extension pour un format.
			switch (format)
			{
				case ExportFormat.Txt:
					return ".txt";

				case ExportFormat.Csv:
					return ".csv";

				case ExportFormat.Xml:
					return ".xml";

				case ExportFormat.Yaml:
					return ".yml";

				case ExportFormat.Json:
					return ".json";

				case ExportFormat.Pdf:
					return ".pdf";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid format", format));
			}
		}

		private static IEnumerable<ExportFormat> Formats
		{
			//	Enumère tous les formats disponibles, par ordre d'importance.
			get
			{
				yield return ExportFormat.Pdf;
				yield return ExportFormat.Txt;
				yield return ExportFormat.Csv;
				yield return ExportFormat.Xml;
				yield return ExportFormat.Yaml;
				yield return ExportFormat.Json;
			}
		}
	}
}