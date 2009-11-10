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

		public override DataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines)
		{
			if (header.Count < 2)
			{
				return null;
			}

			DataCube  cube  = new DataCube ();
			DataTable table = new DataTable ();

			string headerZero = header[0];
			var    headerInfo = headerZero.Split ('/').Select (x => x.Trim ());

			string rowDimension = headerInfo.ElementAtOrDefault (0);
			string colDimension = headerInfo.ElementAtOrDefault (1);
			
			string sourceName = headerInfo.ElementAtOrDefault (2) ?? "CSV";

			table.DimensionVector.Add ("Source", sourceName);
			table.DefineColumnLabels (GraphDataSet.CreateNumberedColumnLabels (header.Skip (1)));
			table.ColumnDimensionKey = colDimension;
			table.RowDimensionKey    = rowDimension;

			using (var mapper = new Mapper<string, string> (labels => GraphDataSet.CreateNumberedColumnLabels (labels)))
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
