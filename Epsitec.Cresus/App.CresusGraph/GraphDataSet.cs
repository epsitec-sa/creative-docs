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

		
		internal static DataTable LoadComptaDemoData()
		{
			CsvFormat format = new CsvFormat ()
			{
				Encoding = System.Text.Encoding.Default,
				FieldSeparator = "\t",
				MultilineSeparator = "\n",
				ColumnNames = new string[] { "Compte", "01¦Jan.", "02¦Fév.", "03¦Mar.", "04¦Avr.", "05¦Mai", "06¦Juin", "07¦Juil.", "08¦Août", "09¦Sep.", "10¦Oct.", "11¦Nov.", "12¦Déc." }
			};

			var reader = new System.IO.MemoryStream (System.Text.Encoding.Default.GetBytes (
@"Ventes Compta	58'657.35	37'380.09	44'223.75	31'909.99	27'961.05	25'380.22	29'917.34	24'180.40				
Ventes Facturation	34'169.26	23'487.28	20'944.26	6'332.75	16'763.03	11'024.99	11'247.38	14'672.91				
Ventes Salaires	40'753.72	26'552.07	15'210.08	11'242.62	7'527.91	9'760.20	10'448.41	7'985.12				
Màj Compta	13'377.05	11'869.62	9'153.44	7'575.97	4'680.60	5'992.15	7'239.11	6'245.42				
Màj Facturation	6'203.84	3'533.78	3'545.02	1'163.22	3'461.93	2'451.06	4'171.32	1'744.84				
Màj Salaires	35'921.16	8'419.46	6'563.86	2'234.12	3'711.18	3'098.45	3'658.94	2'492.59				"));

			var dataTable = CsvReader.ReadCsv (reader, format);
			var table = DataTable.LoadFromData (dataTable,
					columns => columns.Skip (1),
					row =>
					{
						string name = (string) row[0];
						IEnumerable<double?> values = row.ItemArray.Skip (1).Select (x => GraphDataSet.GetNumericValue (x));
						return new KeyValuePair<string, IEnumerable<double?>> (name, values);
					});

			return table;
		}
		
		
		public static double? GetNumericValue(object x)
		{
			double value;
			double sign = 1.0;
			string text = x as string;

			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			text = text.Replace ("'", "");

			if ((text.StartsWith ("(")) &&
				(text.EndsWith (")")))
			{
				text = text.Substring (1, text.Length-2);
				sign = -1.0;
			}

			if (double.TryParse (text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
			{
				return value * sign;
			}

			return null;
		}

		public static IEnumerable<string> CreateNumberedLabels(IEnumerable<string> labels)
		{
			return GraphDataSet.CreateNumberedLabels (labels, index => index);
		}

		public static IEnumerable<string> CreateNumberedLabels(IEnumerable<string> labels, System.Func<int, int> indexMapper)
		{
			int index = 0;

			foreach (string label in labels)
			{
				yield return GraphDataSet.CreateNumberedLabel (label, indexMapper (index++));
			}
		}

		public static string CreateNumberedLabel(string label, int index)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:000}{1}{2}", index, DataCube.LabelSortPrefixSeparator, label);
		}

		public event EventHandler Changed;

		private DataTable dataTable;
	}
}
