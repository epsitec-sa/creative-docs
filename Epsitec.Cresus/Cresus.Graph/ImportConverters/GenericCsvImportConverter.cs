//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>CsvImportConverter</c> class converts from a generic
	/// CSV format.
	/// </summary>
	[Importer ("generic:csv", Priority=20)]
	public class GenericCsvImportConverter : AbstractImportConverter
	{
		public GenericCsvImportConverter(string name)
			: base (name)
		{
		}

		public override AbstractImportConverter CreateSpecificConverter(IDictionary<string, string> meta)
		{
			return new GenericCsvImportConverter (this.Name)
			{
				Meta = meta
			};
		}

		public override bool CheckCompatibleMeta(IDictionary<string, string> meta)
		{
			return meta.Count == 0;
		}

		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta)
		{
			if (header.Count < 2)
			{
				return null;
			}

			var cube  = new GraphDataCube ();
			var table = new DataTable ();

			string headerZero = header[0];
			var    headerInfo = headerZero.Split ('/').Select (x => x.Trim ()).Where (x => x.Length > 0).ToArray ();

			string sourceName   = "CSV";
			string rowDimension = "Ligne";
			string colDimension = "Colonne";

			switch (headerInfo.Length)
            {
				case 0:
					break;

				case 1:
					sourceName = headerInfo[0];
					break;

				case 2:
					rowDimension = headerInfo[0];
					colDimension = headerInfo[1];
					break;

				default:
					rowDimension = headerInfo[0];
					colDimension = headerInfo[1];
					sourceName   = headerInfo[2];
					break;
            }

			table.DimensionVector.Add ("Source", sourceName);
			table.DefineColumnLabels (GraphDataSet.CreateNumberedLabels (header.Skip (1)));
			table.ColumnDimensionKey = colDimension;
			table.RowDimensionKey    = rowDimension;

			using (var mapper = new Mapper<string, string> (labels => GraphDataSet.CreateNumberedLabels (labels)))
			{
				foreach (var line in lines)
				{
					var label  = mapper.Map (line.ElementAt (0));
					var values = line.Skip (1).Select (x => GraphDataSet.GetNumericValue (x));

					table.Add (label, values);
				}
			}

			cube.AddTable (table);

			return cube;
		}

		public override Epsitec.Common.Widgets.Command PreferredGraphType
		{
			get
			{
				return Res.Commands.GraphType.UseLineChart;
			}
		}

		public override string DataTitle
		{
			get
			{
				return "CSV Générique";
			}
		}

		public override GraphDataCategory GetCategory(ChartSeries series)
		{
			return base.GetCategory (series);
		}
	}
}
