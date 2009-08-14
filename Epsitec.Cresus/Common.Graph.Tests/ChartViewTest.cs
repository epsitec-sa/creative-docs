//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Graph
{
	[TestFixture]
	public class ChartViewTest
	{
		[SetUp]
		public void Initialize()
		{
			Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			//	Si ce test est exécuté avant les autres tests, ceux-ci ne bloquent pas
			//	dans l'interaction des diverses fenêtres. Utile si on fait un [Run] de
			//	tous les tests d'un coup.

			Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckRenderer()
		{
			Window window = new Window ();

			double width = 550;
			double height = 500;

			window.ClientSize = new Size (width, height);
			window.Text = string.Concat ("CheckRenderer");
			window.Root.Padding = new Margins (10, 10, 10, 10);

			Data.ChartSeries chartSeries1 = new Data.ChartSeries ()
			{
				Label = "Ventes 2008"
			};

			chartSeries1.Values.Add (new Data.ChartValue ("Jan", 120));
			//			chartSeries1.Values.Add (new Data.ChartValue ("Fév", 160));
			chartSeries1.Values.Add (new Data.ChartValue ("Mar", 80));
			chartSeries1.Values.Add (new Data.ChartValue ("Apr", 70));

			Data.ChartSeries chartSeries2 = new Data.ChartSeries ()
			{
				Label = "Ventes 2009"
			};


			chartSeries2.Values.Add (new Data.ChartValue ("Jan", 70));
			chartSeries2.Values.Add (new Data.ChartValue ("Fév", 50));
			chartSeries2.Values.Add (new Data.ChartValue ("Mar", 40));
			chartSeries2.Values.Add (new Data.ChartValue ("Apr", 130));

			Data.ChartSeries chartSeries3 = new Data.ChartSeries ()
			{
				Label = "Budget 2010"
			};

			chartSeries3.Values.Add (new Data.ChartValue ("Jan", 20));
			chartSeries3.Values.Add (new Data.ChartValue ("Fév", 50));
			chartSeries3.Values.Add (new Data.ChartValue ("Mar", 110));
			chartSeries3.Values.Add (new Data.ChartValue ("Apr", 100));


			Renderers.LineChartRenderer lineChartRenderer = new Renderers.LineChartRenderer ();

			lineChartRenderer.Clear ();
			lineChartRenderer.Collect (chartSeries1);
			lineChartRenderer.Collect (chartSeries2);
			lineChartRenderer.Collect (chartSeries3);
			lineChartRenderer.ClipRange (System.Math.Min (0, lineChartRenderer.MinValue), System.Math.Max (0, lineChartRenderer.MaxValue));
			lineChartRenderer.AddStyle (new Styles.ColorStyle ("line-color") { "Red", "Green", "Blue" });
			lineChartRenderer.AddAdorner (new Adorners.CoordinateAxisAdorner ());

			Widgets.ChartView chartView = new Widgets.ChartView ()
			{
				Dock = DockStyle.Fill
			};

			chartView.DefineRenderer (lineChartRenderer);

			window.Root.Children.Add (chartView);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckViewComptaData()
		{
			IO.CsvFormat format = new Epsitec.Common.IO.CsvFormat ()
			{
				Encoding = System.Text.Encoding.Default,
				FieldSeparator = "\t",
				MultilineSeparator = "\n",
				ColumnNames = new string[] { "Numéro", "Titre du compte", "01/Jan.", "02/Fév.", "03/Mar.", "04/Avr.", "05/Mai", "06/Juin", "07/Juil.", "08/Août", "09/Sep.", "10/Oct.", "11/Nov.", "12/Déc." }
			};

			var dataTable = IO.CsvReader.ReadCsv ("Compta - Résumé périodique mensuel.txt", format);
			var table = Data.DataTable.LoadFromData (dataTable,
					columns => columns.Skip (2),
					row =>
					{
						string name = (string) row[0] + " " + (string) row[1];
						IEnumerable<double?> values = row.ItemArray.Skip (2).Select (x => DataTest.GetNumericValue (x));
						return new KeyValuePair<string, IEnumerable<double?>> (name, values);
					});

			DataTest.DumpTable (table);

			Window window = new Window ();

			double width = 550;
			double height = 500;

			window.ClientSize = new Size (width, height);
			window.Text = string.Concat ("CheckViewComptaData");
			window.Root.Padding = new Margins (0, 0, 0, 0);

			Renderers.LineChartRenderer lineChartRenderer = new Renderers.LineChartRenderer ();

			foreach (var series in table.GetRowSeries ())
			{
				System.Console.Out.WriteLine (series.ToString ());
			}

			lineChartRenderer.Clear ();
			lineChartRenderer.CollectRange (table.GetRowSeries ());
			lineChartRenderer.ClipRange (System.Math.Min (0, lineChartRenderer.MinValue), System.Math.Max (0, lineChartRenderer.MaxValue));
			lineChartRenderer.AddStyle (new Styles.ColorStyle ("line-color") { "Red", "Green", "Blue", "Yellow" });
			lineChartRenderer.AddAdorner (new Adorners.CoordinateAxisAdorner ());

			System.Console.Out.WriteLine (string.Join ("\t", lineChartRenderer.ValueLabels.ToArray ()));

			Widgets.ChartView chartView = new Widgets.ChartView ()
			{
				Dock = DockStyle.Fill
			};

			chartView.DefineRenderer (lineChartRenderer);

			window.Root.Children.Add (chartView);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckPrint()
		{
			Data.ChartSeries chartSeries1 = new Data.ChartSeries ();

			chartSeries1.Values.Add (new Data.ChartValue ("Jan", 120));
			//			chartSeries1.Values.Add (new Data.ChartValue ("Fév", 160));
			chartSeries1.Values.Add (new Data.ChartValue ("Mar", 80));
			chartSeries1.Values.Add (new Data.ChartValue ("Apr", 70));

			Data.ChartSeries chartSeries2 = new Data.ChartSeries ();

			chartSeries2.Values.Add (new Data.ChartValue ("Jan", 70));
			chartSeries2.Values.Add (new Data.ChartValue ("Fév", 50));
			chartSeries2.Values.Add (new Data.ChartValue ("Mar", 40));
			chartSeries2.Values.Add (new Data.ChartValue ("Apr", 130));


			Renderers.LineChartRenderer lineChartRenderer = new Renderers.LineChartRenderer ();

			lineChartRenderer.Clear ();
			lineChartRenderer.Collect (chartSeries1);
			lineChartRenderer.Collect (chartSeries2);
			lineChartRenderer.ClipRange (System.Math.Min (0, lineChartRenderer.MinValue), System.Math.Max (0, lineChartRenderer.MaxValue));
			lineChartRenderer.AddStyle (new Styles.ColorStyle ("line-color") { "Red", "Green", "Blue" });
			lineChartRenderer.AddAdorner (new Adorners.CoordinateAxisAdorner ());

			PrintDocument document = new PrintDocument ();
			document.SelectPrinter ("Adobe PDF");

			document.Print (new PrintEngine (
				port =>
				{
					lineChartRenderer.Render (new List<Data.ChartSeries> () { chartSeries1, chartSeries2 }, port, Rectangle.Deflate (document.DefaultPageSettings.Bounds, 24, 24));
				}));
		}
		
		class PrintEngine : IPrintEngine
		{
			public PrintEngine(System.Action<PrintPort> print)
			{
				this.print = print;
			}

			#region IPrintEngine Members
			
			public void PrepareNewPage(PageSettings settings)
			{
				settings.Margins = new Margins (0, 0, 0, 0);
			}

			public void FinishingPrintJob()
			{
			}

			public void StartingPrintJob()
			{
			}

			public PrintEngineStatus PrintPage(PrintPort port)
			{
				this.print (port);

				return PrintEngineStatus.FinishJob;
			}
			
			#endregion

			readonly System.Action<PrintPort> print;
		}
	}
}
