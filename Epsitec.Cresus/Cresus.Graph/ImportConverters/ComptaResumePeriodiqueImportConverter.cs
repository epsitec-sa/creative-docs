//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>ComptaResumePeriodiqueImportConverter</c> class converts from the
	/// Crésus Comptabilité report named "Résumé périodique".
	/// </summary>
	[Importer ("compta:résumé périodique")]
	public class ComptaResumePeriodiqueImportConverter : AbstractImportConverter
	{
		public ComptaResumePeriodiqueImportConverter(string name)
			: base (name)
		{
		}

		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath)
		{
			if (header.Count < 2)
			{
				return null;
			}

			if ((header[0] != "Numéro") ||
				(header[1] != "Titre du compte"))
			{
				return null;
			}

			var cube  = new GraphDataCube ();
			var table = new DataTable ();

			string colDimension;
			string rowDimension = "Numéro/Compte";

			switch (header.Count-2)
			{
				case 4:
					colDimension = "Trimestre";
					break;
				
				case 6:
				case 12:
					colDimension = "Mois";
					break;

				default:
					return null;
			}

			string sourceName = "";

			if (string.IsNullOrEmpty (sourcePath))
			{
				//	Unknown source - probably the clipboard
			}
			else
			{
				var fileExt  = System.IO.Path.GetExtension (sourcePath).ToLowerInvariant ();
				var fileName = System.IO.Path.GetFileNameWithoutExtension (sourcePath);

				if ((fileExt == ".cre") &&
					(fileName.Length > 4))
                {
					int year;

					if (int.TryParse (fileName.Substring (fileName.Length-4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out year))
					{
						sourceName = year.ToString ("D4");
					}
                }
			}

			table.DimensionVector.Add ("Source", sourceName);
			table.DefineColumnLabels (GraphDataSet.CreateNumberedLabels (header.Skip (2)));
			table.ColumnDimensionKey = colDimension;
			table.RowDimensionKey    = rowDimension;

			foreach (var line in lines)
			{
				var item0 = line.ElementAt (0);
				var item1 = line.ElementAt (1);

				if (string.IsNullOrEmpty (item0))
				{
					continue;
				}

				int start = 0;

				while (start < item1.Length)
				{
					char c = item1[start];

					if ((c != ' ') &&
						(c != '>'))
					{
						break;
					}
				}

				item1 = item1.Substring (start);

				if (string.IsNullOrEmpty (item1))
				{
					continue;
				}

				string label = string.Concat (item0, " ", item1);
				IEnumerable<double?> values = line.Skip (2).Select (x => GraphDataSet.GetNumericValue (string.IsNullOrEmpty (x) ? "0" : x));

				table.Add (label, values);
			}

			cube.AddTable (table);

			return cube;
		}

		public override string DataTitle
		{
			get
			{
				return "Compta – Résumé périodique";
			}
		}

		public override GraphDataCategory GetCategory(ChartSeries series)
		{
			string cat = series.Label.Substring (0, 1);

			switch (cat)
			{
				case "1":
					return new GraphDataCategory (1, "Actif");

				case "2":
					return new GraphDataCategory (2, "Passif");

				case "3":
				case "7":
					return new GraphDataCategory (3, "Produit");

				case "4":
				case "5":
				case "6":
				case "8":
					return new GraphDataCategory (4, "Charge");

				case "9":
					return new GraphDataCategory (5, "Exploitation");
			}
			
			return base.GetCategory (series);
		}
	}
}
