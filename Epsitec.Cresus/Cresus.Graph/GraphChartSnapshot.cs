//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Data;
using System.Xml.Linq;
using System;

namespace Epsitec.Cresus.Graph
{
	public class GraphChartSnapshot
	{
		protected GraphChartSnapshot()
		{
			this.seriesItems = new List<ChartSeries> ();
			this.columns = new List<string> ();
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


		internal XElement SaveSettings(XElement xml)
		{
			xml.Add (GraphChartSnapshot.SaveColorStyle (this.ColorStyle));
			xml.Add (new XElement ("items", this.SeriesItems.Select (x => GraphChartSnapshot.SaveSeries (x))));
			xml.Add (new XElement ("columnLabels", this.ColumnLabels.Select (x => GraphChartSnapshot.SaveColumnLabel (x))));

			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.colorStyle = new ColorStyle (null);
			this.colorStyle.RestoreSettings (xml.Element ("colorStyle"));

			foreach (var element in xml.Elements ("items"))
			{
				var series = new ChartSeries ();
				series.RestoreSettings (element);
				this.seriesItems.Add (series);
			}
			foreach (var element in xml.Elements ("columnLabels"))
			{
				this.columns.Add ((string) element.Attribute ("value"));
			}
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
        


		public static GraphChartSnapshot FromDocument(GraphDocument document)
		{
			var snapshot = new GraphChartSnapshot ();

			snapshot.colorStyle = GraphChartSnapshot.CreateChartSeriesColorStyle (document);
			snapshot.seriesItems.AddRange (GraphChartSnapshot.CreateChartSeriesCollection (document));
			snapshot.columns.AddRange (GraphChartSnapshot.CreateChartSeriesColumnLabels (document));

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
