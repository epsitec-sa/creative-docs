//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Epsitec.Common.Graph
{
	[TestFixture]
	public class DataTest
	{
		[Test]
		public void CheckDataCube()
		{
			Data.DataTable table = new Data.DataTable ()
			{
				RowDimensionKey = "Produit",
				ColumnDimensionKey = "Mois"
			};

			table.DimensionVector.Add ("Année", "2008");
			table.DimensionVector.Add ("Vendeur", "Jean Dupont");

			table.Add ("Facturation", new double?[] { 123,  99, 57, 41, 12,  4,  3, 15,    35, 56, 78, 41 });
			table.Add ("Salaires",    new double?[] { 221, 102, 56, 49, 38, 27, 23, 19,  null,  3,  0, 25 });
			table.Add ("Compta",      new double?[] { 151,  87, 69, 72, 56, 32, 19, 55,    61, 48, 44, 37 });

			table.ColumnLabels.Add ("01/Janvier");
			table.ColumnLabels.Add ("02/Février");
			table.ColumnLabels.Add ("03/Mars");
			table.ColumnLabels.Add ("04/Avril");
			table.ColumnLabels.Add ("05/Mai");
			table.ColumnLabels.Add ("06/Juin");
			table.ColumnLabels.Add ("07/Juillet");
			table.ColumnLabels.Add ("08/Août");
			table.ColumnLabels.Add ("09/Septembre");
			table.ColumnLabels.Add ("10/Octobre");
			table.ColumnLabels.Add ("11/Novembre");
			table.ColumnLabels.Add ("12/Décembre");
			
			Data.DataCube cube = new Data.DataCube ();

			cube.AddTable (table);

			Assert.AreEqual ("Année|Mois|Produit|Vendeur", string.Join ("|", cube.DimensionNames.ToArray ()));

			Assert.AreEqual ("2008", string.Join ("|", cube.GetDimensionValues ("Année").ToArray ()));
			Assert.AreEqual ("01/Janvier|02/Février|03/Mars|04/Avril|05/Mai|06/Juin|07/Juillet|08/Août|09/Septembre|10/Octobre|11/Novembre|12/Décembre", string.Join ("|", cube.GetDimensionValues ("Mois").ToArray ()));
			Assert.AreEqual ("Compta|Facturation|Salaires", string.Join ("|", cube.GetDimensionValues ("Produit").ToArray ()));
			Assert.AreEqual ("Jean Dupont", string.Join ("|", cube.GetDimensionValues ("Vendeur").ToArray ()));

			var series1 = cube.ExtractSeries ("Produit=Facturation", "Mois");
			var series2 = cube.ExtractSeries ("Mois=01/Janvier", "Produit");
			var series3 = cube.ExtractSeries ("Mois=01/Janvier", "Année=2008", "Vendeur");
			var series4 = cube.ExtractSeries ("Produit");
			var series5 = cube.ExtractSeries ("Année=2008", "Mois");
			var series6 = cube.ExtractSeries ("Année=2008", "Mois", "Produit");

			System.Console.Out.WriteLine (series1);
			System.Console.Out.WriteLine (series2);
			System.Console.Out.WriteLine (series3);
			System.Console.Out.WriteLine (series4);
			System.Console.Out.WriteLine (series5);
			System.Console.Out.WriteLine (series6);

			var table1 = cube.ExtractTable ("Année=2008", "Mois", "Produit");	// 12 lignes,  3 colonnes
			var table2 = cube.ExtractTable ("Année=2008", "Produit", "Mois");	//  3 lignes, 12 colonnes

			Assert.AreEqual ( 3, table1.ColumnLabels.Count);
			Assert.AreEqual (12, table1.RowLabels.Count);
			Assert.AreEqual (12, table2.ColumnLabels.Count);
			Assert.AreEqual ( 3, table2.RowLabels.Count);

			DataTest.DumpTable (table1);
			DataTest.DumpTable (table2);
		}

		[Test]
		public void CheckDataTable()
		{
			Data.DataTable table = new Data.DataTable ()
			{
				RowDimensionKey = "Produit",
				ColumnDimensionKey = "Mois"
			};

			table.DimensionVector.Add ("Année", "2008");
			table.DimensionVector.Add ("Vendeur", "Jean Dupont");

			table.Add ("Facturation", new double?[] { 123,  99, 57, 41, 12,  4,  3, 15,    35, 56, 78, 41 });
			table.Add ("Salaires",    new double?[] { 221, 102, 56, 49, 38, 27, 23, 19,  null,  3,  0, 25 });
			table.Add ("Compta",      new double?[] { 151,  87, 69, 72, 56, 32, 19, 55,    61, 48, 44, 37 });

			table.ColumnLabels.Add ("01/Janvier");
			table.ColumnLabels.Add ("02/Février");
			table.ColumnLabels.Add ("03/Mars");
			table.ColumnLabels.Add ("04/Avril");
			table.ColumnLabels.Add ("05/Mai");
			table.ColumnLabels.Add ("06/Juin");
			table.ColumnLabels.Add ("07/Juillet");
			table.ColumnLabels.Add ("08/Août");
			table.ColumnLabels.Add ("09/Septembre");
			table.ColumnLabels.Add ("10/Octobre");
			table.ColumnLabels.Add ("11/Novembre");
			table.ColumnLabels.Add ("12/Décembre");

			var series1 = table.GetRowSeries (0);

			Assert.AreEqual ("Facturation", series1.Label);
			Assert.AreEqual (12, series1.Values.Count);
			Assert.AreEqual ("09/Septembre", series1.Values[8].Label);
			Assert.AreEqual (35.0, series1.Values[8].Value);

			//	Verify that missing values are indeed stripped from the produced chart series :
			
			var series2 = table.GetRowSeries (1);

			Assert.AreEqual ("Salaires", series2.Label);
			Assert.AreEqual (11, series2.Values.Count);
			Assert.AreEqual ("10/Octobre", series2.Values[8].Label);
			Assert.AreEqual (3, series2.Values[8].Value);
		}

		[Test]
		public void CheckDimension()
		{
			Data.Dimension ax = new Data.Dimension ("A", "x");
			Data.Dimension bx = new Data.Dimension ("B", "x");
			Data.Dimension by = new Data.Dimension ("B", "y");

			Assert.AreEqual (true, ax.Equals (ax));
			Assert.AreEqual (false, ax.Equals (bx));

			Assert.AreEqual (0, ax.CompareTo (ax));
			Assert.AreEqual (-1, ax.CompareTo (bx));
			Assert.AreEqual (1, bx.CompareTo (ax));
			
			Assert.AreEqual (0, bx.CompareTo (bx));
			Assert.AreEqual (-1, bx.CompareTo (by));
			Assert.AreEqual (1, by.CompareTo (bx));
		}

		[Test]
		public void CheckDimensionVector()
		{
			Data.DimensionVector vector = new Data.DimensionVector ();

			Assert.AreEqual ("", vector.Compile ());

			vector.Add ("B", "1-b");
			vector.Add ("C", "2-c");
			vector.Add ("A", "3-a");

			Assert.AreEqual ("A=3-a:B=1-b:C=2-c", vector.Compile ());

			var vector1 = new Data.DimensionVector ("A=3-a:B=1-b:C=2-c");
			var vector2 = new Data.DimensionVector (vector);

			Assert.IsTrue (vector.Equals (vector1));
			Assert.IsTrue (vector.Equals (vector2));
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckDimensionVectorEx1()
		{
			Data.DimensionVector vector = new Data.DimensionVector ();

			vector.Add ("B", "1-b");
			vector.Add ("A", "2-a");
			vector.Add ("B", "3-b");
		}

		private static void DumpTable(Epsitec.Common.Graph.Data.DataTable table)
		{
			System.Console.Out.WriteLine ("Lignes: {0}, colonnes: {1}", table.RowDimensionKey, table.ColumnDimensionKey);
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (var row in table.RowLabels)
			{
				buffer.AppendFormat ("{0,-20}", row);

				foreach (var col in table.ColumnLabels)
				{
					double? value = table[row, col];
					buffer.AppendFormat ("\t{0}", value.HasValue ? value.Value.ToString () : "---");
				}

				System.Console.Out.WriteLine (buffer.ToString ());
				buffer.Length = 0;
			}
		}
	}
}
