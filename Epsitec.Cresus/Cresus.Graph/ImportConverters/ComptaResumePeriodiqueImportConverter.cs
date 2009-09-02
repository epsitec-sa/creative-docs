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
	public class ComptaResumePeriodiqueImportConverter : AbstractImportConverter
	{
		public override DataCube ToDataCube(IList<string> header, IEnumerable<IList<string>> lines)
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

			DataCube  cube  = new DataCube ();
			DataTable table = new DataTable ();

			string columnDimension;

			switch (header.Count-2)
			{
				case 4:
					columnDimension = "Trimestre";
					break;
				
				case 6:
				case 12:
					columnDimension = "Mois";
					break;

				default:
					return null;
			}

			table.DefineColumnLabels (GraphDataSet.CreateNumberedColumnLabels (header.Skip (2)));
			table.ColumnDimensionKey = columnDimension;
			table.RowDimensionKey = "Numéro/Compte";

			foreach (var line in lines)
			{
				string label = string.Concat (line[0], " ", line[1]);
				IEnumerable<double?> values = line.Skip (2).Select (x => GraphDataSet.GetNumericValue (x));

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
