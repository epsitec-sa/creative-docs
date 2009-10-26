//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphChartSnapshot</c> class stores a snapshot of the workspace output series,
	/// with the related settings; the snapshot is used to store a graph in a persistent way.
	/// </summary>
	public class GraphChartSnapshot
	{
		protected GraphChartSnapshot()
		{
			this.seriesItems = new List<ChartSeries> ();
			this.columns = new List<string> ();
			this.Guid = System.Guid.NewGuid ();
			this.Visibility = true;
		}


		public ColorStyle ColorStyle
		{
			get
			{
				return this.colorStyle;
			}
		}

		public IEnumerable<ChartSeries> SeriesItems
		{
			get
			{
				return this.seriesItems;
			}
		}

		public IEnumerable<string> ColumnLabels
		{
			get
			{
				return this.columns;
			}
		}

		public System.Guid Guid
		{
			get;
			private set;
		}

		public string GuidName
		{
			get
			{
				return this.Guid.ToString ("D");
			}
		}

		public bool Visibility
		{
			get;
			set;
		}

		public Command GraphType
		{
			get;
			set;
		}


		internal XElement SaveSettings(XElement xml)
		{
			var  graphType = this.GraphType ?? Res.Commands.GraphType.UseLineChart;

			xml.Add (new XAttribute ("guid", this.Guid));
			xml.Add (new XAttribute ("type", graphType.CommandId));
			xml.Add (GraphChartSnapshot.SaveColorStyle (this.ColorStyle));
			xml.Add (new XElement ("items", this.SeriesItems.Select (x => GraphChartSnapshot.SaveSeries (x))));
			xml.Add (new XElement ("columnLabels", this.ColumnLabels.Select (x => GraphChartSnapshot.SaveColumnLabel (x))));

			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.Guid = (System.Guid) xml.Attribute ("guid");
			this.GraphType = Command.Find ((string) xml.Attribute ("type"));

			this.colorStyle = new ColorStyle (null);
			this.colorStyle.RestoreSettings (xml.Element ("colorStyle"));

			var xmlItems = xml.Element ("items");
			var xmlColumnLabels = xml.Element ("columnLabels");
			
			if (xmlItems != null)
			{
				foreach (var element in xmlItems.Elements ())
				{
					var series = new ChartSeries ();
					series.RestoreSettings (element);
					this.seriesItems.Add (series);
				}
			}

			if (xmlColumnLabels != null)
			{
				foreach (var element in xmlColumnLabels.Elements ())
				{
					this.columns.Add ((string) element.Attribute ("value"));
				}
			}
		}

		internal AbstractRenderer CreateRenderer(bool visibleLabels)
		{
			AbstractRenderer renderer = null;
			bool stackValues = false;
			var  graphType   = this.GraphType ?? Res.Commands.GraphType.UseLineChart;

			if (graphType == Res.Commands.GraphType.UseLineChart)
			{
				renderer = new LineChartRenderer ()
				{
					SurfaceAlpha = stackValues ? 1.0 : 0.0
				};
			}
			else if (graphType == Res.Commands.GraphType.UseBarChartVertical)
			{
				renderer = new BarChartRenderer ();
			}

			System.Diagnostics.Debug.Assert (renderer != null);

			var adorner = new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
			{
				GridColor = Color.FromBrightness (0.8),
				VisibleGrid = true,
				VisibleLabels = visibleLabels,
				VisibleTicks = true,
			};

			renderer.AddStyle (this.ColorStyle);
			renderer.AddAdorner (adorner);

			var series = this.SeriesItems;

			renderer.Clear ();
			renderer.ChartSeriesRenderingMode = /*this.StackValues ? ChartSeriesRenderingMode.Stacked : */ChartSeriesRenderingMode.Separate;
			renderer.DefineValueLabels (this.ColumnLabels);
			renderer.CollectRange (series);
			renderer.UpdateCaptions (series);
			renderer.AlwaysIncludeZero = true;
			

			return renderer;
		}




		
		private static XElement SaveColorStyle(ColorStyle colorStyle)
		{
			return colorStyle.SaveSettings (new XElement ("colorStyle"));
		}

		private static XElement SaveSeries(ChartSeries series)
		{
			return series.SaveSettings (new XElement ("series"));
		}

		private static XElement SaveColumnLabel(string columnLabel)
		{
			return new XElement ("label",
				new XAttribute ("value", columnLabel ?? ""));
		}
        


		public static GraphChartSnapshot FromDocument(GraphDocument document, Command graphType)
		{
			var snapshot = new GraphChartSnapshot ();

			snapshot.colorStyle = GraphChartSnapshot.CreateChartSeriesColorStyle (document);
			snapshot.seriesItems.AddRange (GraphChartSnapshot.CreateChartSeriesCollection (document));
			snapshot.columns.AddRange (GraphChartSnapshot.CreateChartSeriesColumnLabels (document));
			snapshot.GraphType = graphType;

			return snapshot;
		}

		public static GraphChartSnapshot FromXml(XElement xml)
		{
			var snapshot = new GraphChartSnapshot ();
			snapshot.RestoreSettings (xml);
			return snapshot;
		}


		private static ColorStyle CreateChartSeriesColorStyle(GraphDocument document)
		{
			var color = document.DefaultColorStyle;
			var style = new ColorStyle (color.Name);
			
			foreach (int index in document.OutputSeries.Select (x => x.ColorIndex))
			{
				style.Add (color[index]);
			}

			return style;
		}

		private static IEnumerable<ChartSeries> CreateChartSeriesCollection(GraphDocument document)
		{
			foreach (var series in document.OutputSeries.Select (x => x.ChartSeries))
			{
				yield return new ChartSeries (series.Label, series.Values);
			}
		}
		
		private static IEnumerable<string> CreateChartSeriesColumnLabels(GraphDocument document)
		{
			return document.ChartColumnLabels;
		}

		
		private ColorStyle colorStyle;
		private readonly List<ChartSeries> seriesItems;
		private readonly List<string> columns;
	}
}
