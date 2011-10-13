//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>ComptaNmcBilanImportConverter</c> class converts from the
	/// Crésus Comptabilité NMC report named "Bilan".
	/// </summary>
	[Importer ("compta:nmc-bilan")]
	public class ComptaNmcBilanImportConverter : Compta
	{
		public ComptaNmcBilanImportConverter(string name)
			: base (name, "6")
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
			return new ComptaNmcBilanImportConverter (this.Name)
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
			string[] sources = new string[4];

			string yearName = Compta.GetSourceName (sourcePath);
			
			sources[0] = Compta.RemoveLineBreaks (Compta.MakeFullDate (header[2], yearName));
			sources[1] = Compta.RemoveLineBreaks (Compta.MakeFullDate (header[3], yearName));
			sources[2] = Compta.RemoveLineBreaks (Compta.MakeFullDate (header[4], yearName));
			sources[3] = Compta.RemoveLineBreaks (Compta.MakeFullDate (header[5], yearName));

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
					string value = line.ElementAt (column);
					
					table.Add (label, new double?[] { GraphDataSet.GetNumericValue (string.IsNullOrEmpty (value) ? "0" : value) });
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
				return "Compta – Bilan";
			}
		}
	}
}
