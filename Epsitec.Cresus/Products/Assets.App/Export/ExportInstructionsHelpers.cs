//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
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
					return Res.Strings.Enum.ExportFormat.Txt.ToString ();

				case ExportFormat.Csv:
					return Res.Strings.Enum.ExportFormat.Csv.ToString ();

				case ExportFormat.Xml:
					return Res.Strings.Enum.ExportFormat.Xml.ToString ();

				case ExportFormat.Yaml:
					return Res.Strings.Enum.ExportFormat.Yaml.ToString ();

				case ExportFormat.Json:
					return Res.Strings.Enum.ExportFormat.Json.ToString ();

				case ExportFormat.Pdf:
					return Res.Strings.Enum.ExportFormat.Pdf.ToString ();

				default:
					return null;
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
					throw new System.InvalidOperationException (string.Format ("Invalid format {0}", format));
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