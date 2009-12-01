//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	/// <summary>
	/// The <c>ComptaCompteImportConverter</c> class converts from the
	/// Crésus Comptabilité report named "Compte".
	/// </summary>
	[Importer ("compta:compte")]
	public class ComptaCompteImportConverter : Compta
	{
		public ComptaCompteImportConverter(string name)
			: base (name, "3")
		{
		}

		public override Epsitec.Common.Widgets.Command PreferredGraphType
		{
			get
			{
				return Res.Commands.GraphType.UseLineChart;
			}
		}
		
		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath)
		{
			if (header.Count < 4)
			{
				return null;
			}

			var cube  = new GraphDataCube ();

			string colDimension = "Colonne";
			string rowDimension = "Transaction";
			string sourceName   = Compta.GetSourceName (sourcePath);

			var table = new DataTable ();

			table.DimensionVector.Add ("Source", sourceName);
			table.ColumnDimensionKey = colDimension;
			table.RowDimensionKey    = rowDimension;

			var dates = new HashSet<System.DateTime> ();
			var data  = new Dictionary<System.DateTime, double> ();

			foreach (var line in lines)
			{
				var fields = line.ToArray ();
				
				if (string.IsNullOrEmpty (fields[0]) ||
					string.IsNullOrEmpty (fields[1]))
				{
					continue;
				}

				var date    = Compta.ParseDate (fields[0]);
				var cp      = fields[1];
				var pièce   = fields[2];
				var libellé = fields[3];
				var débit   = GraphDataSet.GetNumericValue (string.IsNullOrEmpty (fields[4]) ? "0" : fields[4]);
				var crédit  = GraphDataSet.GetNumericValue (string.IsNullOrEmpty (fields[5]) ? "0" : fields[5]);
				var solde   = GraphDataSet.GetNumericValue (string.IsNullOrEmpty (fields[6]) ? "0" : fields[6]);

				dates.Add (date);

				if (solde.HasValue)
				{
					data[date] = solde.Value;
				}
			}

			var minDate = dates.Min ();
			var maxDate = dates.Max ();

			var     currentDate  = minDate;
			double? currentSolde = null;
			var     values       = new List<double?> ();
			var     columns      = new List<string> ();

			while (currentDate <= maxDate)
			{
				double value;
				
				if (data.TryGetValue (currentDate, out value))
				{
					currentSolde = value;
				}

				columns.Add (currentDate.ToShortDateString ());
				values.Add (currentSolde);

				currentDate = currentDate.AddDays (1);
			}

			table.DefineColumnLabels (GraphDataSet.CreateNumberedLabels (columns));
			table.Add ("Compte", values);

			cube.AddTable (table);

			return cube;
		}

		public override string DataTitle
		{
			get
			{
				return "Compta – Extraits de comptes";
			}
		}
	}
}
