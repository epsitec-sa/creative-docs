//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;
using Epsitec.Common.IO;
using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataSet
	{
		public GraphDataSet()
		{
		}


		public DataTable DataTable
		{
			get
			{
				return this.dataTable;
			}
		}


		public void LoadDataTable()
		{
			this.LoadDataTable (GraphDataSet.LoadComptaData ());
		}

		public void LoadDataTable(DataTable table)
		{
			this.dataTable = table;
			this.OnChanged ();
		}

		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		internal static DataTable LoadComptaData()
		{
			CsvFormat format = new CsvFormat ()
			{
				Encoding = System.Text.Encoding.Default,
				FieldSeparator = "\t",
				MultilineSeparator = "\n",
				ColumnNames = new string[] { "Numéro", "Titre du compte", "01¦Jan.", "02¦Fév.", "03¦Mar.", "04¦Avr.", "05¦Mai", "06¦Juin", "07¦Juil.", "08¦Août", "09¦Sep.", "10¦Oct.", "11¦Nov.", "12¦Déc." }
			};

			var dataTable = CsvReader.ReadCsv (@"S:\Epsitec.Cresus\Common.Graph.Tests\DataSamples\Compta - Résumé périodique mensuel (complet).txt", format);
			var table = DataTable.LoadFromData (dataTable,
					columns => columns.Skip (2),
					row =>
					{
						string name = (string) row[0] + " " + (string) row[1];
						IEnumerable<double?> values = row.ItemArray.Skip (2).Select (x => GraphDataSet.GetNumericValue (x));
						return new KeyValuePair<string, IEnumerable<double?>> (name, values);
					});

			return table;
		}
		
		
		public static double? GetNumericValue(object x)
		{
			double value;
			string text = x as string;

			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}

			text = text.Replace ("'", "");

			if (double.TryParse (text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
			{
				return value;
			}

			return 0;
		}

		public static IEnumerable<string> CreateNumberedColumnLabels(IEnumerable<string> labels)
		{
			int index = 0;

			foreach (string label in labels)
			{
				yield return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:000}{1}{2}", index++, DataCube.LabelSortPrefixSeparator, label);
			}
		}

		public event EventHandler Changed;

		private DataTable dataTable;
	}
}
