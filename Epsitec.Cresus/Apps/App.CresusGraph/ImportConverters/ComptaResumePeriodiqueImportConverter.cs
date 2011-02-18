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
	public class ComptaResumePeriodiqueImportConverter : Compta
	{
		public ComptaResumePeriodiqueImportConverter(string name)
			: base (name, "10")
		{
		}


		public override Epsitec.Common.Widgets.Command PreferredGraphType
		{
			get
			{
				return Res.Commands.GraphType.UseLineChart;
			}
		}

		public override AbstractImportConverter CreateSpecificConverter(IDictionary<string, string> meta)
		{
			return new ComptaResumePeriodiqueImportConverter (this.Name)
			{
				Meta = meta
			};
		}

		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta)
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

			string colDimension = "Période";
			string rowDimension = "Numéro/Compte";
			string sourceName   = Compta.GetSourceName (sourcePath);

			table.DimensionVector.Add ("Source", sourceName);
			table.DefineColumnLabels (GraphDataSet.CreateNumberedLabels (header.Skip (2).Where (x => x != "Précédent")));	// contourne bug du "Précédent" (provisoire)
			table.ColumnDimensionKey = colDimension;
			table.RowDimensionKey    = rowDimension;

			foreach (var line in lines)
			{
				var item0 = line.ElementAt (0);
				var item1 = line.ElementAt (1);

				if (string.IsNullOrEmpty (item0) &&
					string.IsNullOrEmpty (item1))
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

					start++;
				}

				item1 = item1.Substring (start);

				if (string.IsNullOrEmpty (item1))
				{
					continue;
				}

				string label = string.Concat (item0, "\t", item1);
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
	}
}
