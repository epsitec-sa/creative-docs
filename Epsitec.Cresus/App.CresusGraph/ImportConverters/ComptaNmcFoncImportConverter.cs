//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>ComptaNmcFoncImportConverter</c> class converts from the
	/// Crésus Comptabilité NMC report named "Compte de fonctionnement".
	/// </summary>
	[Importer ("compta:nmc-fonc")]
	public class ComptaNmcFoncImportConverter : Compta
	{
		public ComptaNmcFoncImportConverter(string name)
			: base (name, "7")
		{
		}

		public override Epsitec.Common.Widgets.Command PreferredGraphType
		{
			get
			{
				return Res.Commands.GraphType.UseBarChartVertical;
			}
		}

		public override AbstractImportConverter  CreateSpecificConverter(IDictionary<string,string> meta)
		{
			return new ComptaNmcFoncImportConverter (this.Name)
			{
				Meta = meta
			};
		}

		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta)
		{
			if (header.Count < 4)
			{
				return null;
			}

			var cube  = new GraphDataCube ();

			string colDimension = "Colonne";
			string rowDimension = "Numéro/Compte";
			string[] sources = new string[header.Count-2];

			string yearName = Compta.GetSourceName (sourcePath);

			for (int i = 0; i < sources.Length; i++)
			{
				sources[i] = Compta.RemoveLineBreaks (Compta.MakeFullDate (header[2+i], yearName));
			}

			string baseNumber = "";

			int column = 2;

			foreach (var sourceName in GraphDataSet.CreateNumberedLabels (sources, index => index))
			{
				var table = new DataTable ();

				table.DimensionVector.Add ("Source", sourceName);
				table.DefineColumnLabels (new string[] { "Solde" });
				table.ColumnDimensionKey = colDimension;
				table.RowDimensionKey    = rowDimension;

				foreach (var line in lines)
				{
					var item0 = line.ElementAt (0);
					var item1 = line.ElementAt (1);

					if (string.IsNullOrEmpty (item0) ||
						string.IsNullOrEmpty (item1))
					{
						continue;
					}

					if (item0.StartsWith (" "))
					{
						item0 = string.Concat (baseNumber, ".", item0.Trim ());
					}
					else
					{
						baseNumber = item0;
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

					if (line.Skip (2).Any (x => x.Length > 0))
					{
						string label = string.Concat (item0, "\t", item1);
						string value = line.ElementAt (column);

						table.Add (label, new double?[] { GraphDataSet.GetNumericValue (string.IsNullOrEmpty (value) ? "0" : value) });
					}
				}

				cube.AddTable (table);
				
				if (GraphSerial.LicensingInfo < LicensingInfo.ValidPro)
				{
					break;
				}

				column++;
			}

			return cube;
		}

		public override string DataTitle
		{
			get
			{
				return "Compta – Compte de fonctionnement";
			}
		}
	}
}
