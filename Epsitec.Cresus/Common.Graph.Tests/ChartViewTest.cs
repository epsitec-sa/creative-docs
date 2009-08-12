using System.Collections.Generic;

using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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

			double width = 400;
			double height = 600;

			window.ClientSize = new Size (width, height);
			window.Text = string.Concat ("CheckRenderer");
			window.Root.Padding = new Margins (10, 10, 10, 10);

			Data.ChartSeries chartSeries1 = new Data.ChartSeries ();

			chartSeries1.Values.Add (new Data.ChartValue ("Jan", 120));
			chartSeries1.Values.Add (new Data.ChartValue ("Fév", 160));
			chartSeries1.Values.Add (new Data.ChartValue ("Mar",  80));

			Renderers.LineChartRenderer lineChartRenderer = new Renderers.LineChartRenderer ();

			lineChartRenderer.Clear ();
			lineChartRenderer.Collect (chartSeries1);
			lineChartRenderer.ClipRange (System.Math.Min (0, lineChartRenderer.MinValue), System.Math.Max (0, lineChartRenderer.MaxValue));

			Widgets.ChartView chartView = new Widgets.ChartView ()
			{
				Dock = DockStyle.Fill
			};

			chartView.DefineRenderer (lineChartRenderer);
			chartView.Items.Add (chartSeries1);

			window.Root.Children.Add (chartView);
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
	}
}
