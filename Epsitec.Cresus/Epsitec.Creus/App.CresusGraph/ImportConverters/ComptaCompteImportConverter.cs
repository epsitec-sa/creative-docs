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

		public override AbstractImportConverter CreateSpecificConverter(IDictionary<string, string> meta)
		{
			return new ComptaCompteImportConverter (this.Name)
			{
				Meta = meta
			};
		}

		public override GraphDataCube ToDataCube(IList<string> header, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta)
		{
//-			System.Threading.Thread.Sleep (10*1000);

			IEnumerable<string>[] headerEnum = new  IEnumerable<string>[] { header };
			var allLines = headerEnum.Concat (lines);

			var cube  = new GraphDataCube ();

			string colDimension = "Colonne";
			string rowDimension = "Transaction";
			string sourceName   = Compta.GetSourceName (sourcePath);

			var enumerator = allLines.GetEnumerator ();
			var dates = new HashSet<System.DateTime> ();
			var list = new List<Compte> ();

			if ((meta != null) &&
				(meta.ContainsKey ("Account")))
			{
				var name   = meta["Account"].Replace ('\t', ' ');
				var compte = ComptaCompteImportConverter.GetData (name, enumerator, dates);

				if (compte.Values.Count > 0)
				{
					list.Add (compte);
				}
			}
			else
			{
				bool skip = false;
				
				while (skip || enumerator.MoveNext ())
				{
					var line   = enumerator.Current;
					var fields = line.ToArray ();
					var field0 = fields[0];

					if (field0.StartsWith ("Compte "))
					{
						var name = field0.Substring (7);
						var compte = ComptaCompteImportConverter.GetData (name, enumerator, dates);
						skip = true;

						if (compte.Values.Count > 0)
						{
							list.Add (compte);
						}
					}
					else
					{
						skip = false;
					}
				}
			}

			var minDate = dates.Min ();
			var maxDate = dates.Max ();
			var dateRange = ComptaCompteImportConverter.Range (minDate, maxDate);

			foreach (var compte in list)
			{
				var table = new DataTable ();

				table.DimensionVector.Add ("Source", sourceName);
				table.ColumnDimensionKey = colDimension;
				table.RowDimensionKey    = rowDimension;
				table.DefineColumnLabels (GraphDataSet.CreateNumberedLabels (dateRange.Select (date => date.ToShortDateString ())));

				table.Add (compte.Name, compte.GetValues (dateRange));

				cube.AddTable (table);
			}
			
			return cube;
		}

		class Compte
		{
			public Compte(string name)
			{
				this.values = new Dictionary<System.DateTime, double> ();
				this.name   = name;
			}

			public Dictionary<System.DateTime, double> Values
			{
				get
				{
					return this.values;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public IEnumerable<double?> GetValues(IEnumerable<System.DateTime> dates)
			{
				double? currentSolde = null;
				
				foreach (var currentDate in dates)
				{
					double value;

					if (this.values.TryGetValue (currentDate, out value))
					{
						currentSolde = value;
					}

					yield return currentSolde;
				}
			}

			private readonly Dictionary<System.DateTime, double> values;
			private readonly string name;
		}

		private static IEnumerable<System.DateTime> Range(System.DateTime minDate, System.DateTime maxDate)
		{
			var currentDate = minDate;

			while (currentDate <= maxDate)
			{
				yield return currentDate;
				currentDate = currentDate.AddDays (1);
			}
		}

		private static Compte GetData(string name, IEnumerator<IEnumerable<string>> enumerator, HashSet<System.DateTime> dates)
		{
			var data = new Compte (name);

			while (enumerator.MoveNext ())
			{
				var line   = enumerator.Current;
				var fields = line.ToArray ();

				if ((fields.Length < 7) ||
					(string.IsNullOrEmpty (fields[0])) ||
					(string.IsNullOrEmpty (fields[1])) ||
					(fields[0].Length < 8) ||
					(fields[0][2] != '.') ||
					(fields[0][5] != '.'))
				{
					if (fields.Length == 1)
					{
						break;
					}

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
					data.Values[date] = solde.Value;
				}
			}
			
			return data;
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
