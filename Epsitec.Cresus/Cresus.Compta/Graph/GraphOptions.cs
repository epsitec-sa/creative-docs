//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphOptions
	{
		public GraphOptions()
		{
			this.primaryFilter   = new List<FormattedText> ();
			this.secondaryFilter = new List<FormattedText> ();

			this.Clear ();
		}

		public void Clear()
		{
			this.mode               = GraphMode.SideBySide;
			this.style              = GraphStyle.Rainbow;
			this.marginsAbs         = new Margins (10, 10, 30, 10);
			this.titleText          = "Titre";
			this.titlePositionRel   = new Point (0.5, 1.0);
			this.titleSizeAbs       = new Size (400, 30);
			this.startAtZero        = true;
			this.graphPoints        = GraphPoint.Circle;
			this.hasLines           = true;
			this.hasLegend          = true;
			this.legendPositionRel  = new Point (0.99, 0.98);  // en haut à droite
			this.legendColumns      = 1;
			this.hasThreshold0      = false;
			this.hasThreshold1      = false;
			this.thresholdValue0    = 0.05m;  // 5%
			this.thresholdValue1    = 0.05m;  // 5%
			this.fontSize           = 10.0;
			this.borderThickness    = 1.0;
			this.barThickness       = 0.8;
			this.barOverlap         = 0.0;
			this.lineAlpha          = 1.0;
			this.lineWidth          = 2.0;
			this.pointWidth         = 4.0;
			this.explodedPieFactor  = 1.0;
			this.piePercents        = true;
			this.pieValues          = false;
			this.primaryDimension   = 1;
			this.secondaryDimension = 0;

			this.primaryFilter.Clear ();
			this.secondaryFilter.Clear ();
		}


		public GraphMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		public GraphStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
			}
		}

		public Margins MarginsAbs
		{
			get
			{
				return this.marginsAbs;
			}
			set
			{
				this.marginsAbs = value;
			}
		}

		public FormattedText TitleText
		{
			get
			{
				return this.titleText;
			}
			set
			{
				this.titleText = value;
			}
		}

		public Point TitlePositionRel
		{
			get
			{
				return this.titlePositionRel;
			}
			set
			{
				this.titlePositionRel = value;
			}
		}

		public Size TitleSizeAbs
		{
			get
			{
				return this.titleSizeAbs;
			}
			set
			{
				this.titleSizeAbs = value;
			}
		}

		public bool StartAtZero
		{
			get
			{
				return this.startAtZero;
			}
			set
			{
				this.startAtZero = value;
			}
		}

		public bool HasThreshold0
		{
			get
			{
				return this.hasThreshold0;
			}
			set
			{
				this.hasThreshold0 = value;
			}
		}

		public bool HasThreshold1
		{
			get
			{
				return this.hasThreshold1;
			}
			set
			{
				this.hasThreshold1 = value;
			}
		}

		public decimal ThresholdValue0
		{
			get
			{
				return this.thresholdValue0;
			}
			set
			{
				this.thresholdValue0 = value;
			}
		}

		public decimal ThresholdValue1
		{
			get
			{
				return this.thresholdValue1;
			}
			set
			{
				this.thresholdValue1 = value;
			}
		}

		public GraphPoint GraphPoints
		{
			get
			{
				return this.graphPoints;
			}
			set
			{
				this.graphPoints = value;
			}
		}

		public bool HasLines
		{
			get
			{
				return this.hasLines;
			}
			set
			{
				this.hasLines = value;
			}
		}

		public bool HasLegend
		{
			get
			{
				return this.hasLegend;
			}
			set
			{
				this.hasLegend = value;
			}
		}

		public Point LegendPositionRel
		{
			get
			{
				return this.legendPositionRel;
			}
			set
			{
				this.legendPositionRel = value;
			}
		}

		public int LegendColumns
		{
			get
			{
				return this.legendColumns;
			}
			set
			{
				this.legendColumns = value;
			}
		}

		public Point TempDraggedColumnPos
		{
			get;
			set;
		}


		public double FontSize
		{
			get
			{
				return this.fontSize;
			}
			set
			{
				this.fontSize = value;
			}
		}

		public double BorderThickness
		{
			get
			{
				return this.borderThickness;
			}
			set
			{
				this.borderThickness = value;
			}
		}

		public double BarThickness
		{
			get
			{
				return this.barThickness;
			}
			set
			{
				this.barThickness = value;
			}
		}

		public double BarOverlap
		{
			get
			{
				return this.barOverlap;
			}
			set
			{
				this.barOverlap = value;
			}
		}

		public double LineAlpha
		{
			get
			{
				return this.lineAlpha;
			}
			set
			{
				this.lineAlpha = value;
			}
		}

		public double LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				this.lineWidth = value;
			}
		}

		public double PointWidth
		{
			get
			{
				return this.pointWidth;
			}
			set
			{
				this.pointWidth = value;
			}
		}

		public double ExplodedPieFactor
		{
			get
			{
				return this.explodedPieFactor;
			}
			set
			{
				this.explodedPieFactor = value;
			}
		}

		public bool PiePercents
		{
			get
			{
				return this.piePercents;
			}
			set
			{
				this.piePercents = value;
			}
		}

		public bool PieValues
		{
			get
			{
				return this.pieValues;
			}
			set
			{
				this.pieValues = value;
			}
		}


		public int PrimaryDimension
		{
			get
			{
				return this.primaryDimension;
			}
			set
			{
				this.primaryDimension = value;
			}
		}

		public int SecondaryDimension
		{
			get
			{
				return this.secondaryDimension;
			}
			set
			{
				this.secondaryDimension = value;
			}
		}

		public List<FormattedText> PrimaryFilter
		{
			get
			{
				return this.primaryFilter;
			}
		}

		public List<FormattedText> SecondaryFilter
		{
			get
			{
				return this.secondaryFilter;
			}
		}


		public void CopyTo(GraphOptions dst)
		{
			dst.Mode               = this.Mode;
			dst.Style              = this.Style;
			dst.MarginsAbs         = this.MarginsAbs;
			dst.TitleText          = this.TitleText;
			dst.TitlePositionRel   = this.TitlePositionRel;
			dst.TitleSizeAbs       = this.TitleSizeAbs;
			dst.StartAtZero        = this.StartAtZero;
			dst.HasThreshold0      = this.HasThreshold0;
			dst.HasThreshold1      = this.HasThreshold1;
			dst.ThresholdValue0    = this.ThresholdValue0;
			dst.ThresholdValue1    = this.ThresholdValue1;
			dst.GraphPoints        = this.GraphPoints;
			dst.HasLines           = this.HasLines;
			dst.HasLegend          = this.HasLegend;
			dst.LegendPositionRel  = this.LegendPositionRel;
			dst.LegendColumns      = this.LegendColumns;
			dst.FontSize           = this.FontSize;
			dst.BorderThickness    = this.BorderThickness;
			dst.BarThickness       = this.BarThickness;
			dst.BarOverlap         = this.BarOverlap;
			dst.lineAlpha          = this.lineAlpha;
			dst.LineWidth          = this.LineWidth;
			dst.PointWidth         = this.PointWidth;
			dst.ExplodedPieFactor  = this.ExplodedPieFactor;
			dst.PiePercents        = this.PiePercents;
			dst.PieValues          = this.PieValues;
			dst.PrimaryDimension   = this.PrimaryDimension;
			dst.SecondaryDimension = this.SecondaryDimension;

			dst.primaryFilter.Clear ();
			dst.primaryFilter.AddRange (this.primaryFilter);

			dst.secondaryFilter.Clear ();
			dst.secondaryFilter.AddRange (this.secondaryFilter);
		}

		public bool CompareTo(GraphOptions other)
		{
			return this.Mode               == other.Mode               &&
				   this.Style              == other.Style              &&
				   this.MarginsAbs         == other.MarginsAbs         &&
				   this.TitleText          == other.TitleText          &&
				   this.TitlePositionRel   == other.TitlePositionRel   &&
				   this.TitleSizeAbs       == other.TitleSizeAbs       &&
				   this.StartAtZero        == other.StartAtZero        &&
				   this.HasThreshold0      == other.HasThreshold0      &&
				   this.HasThreshold1      == other.HasThreshold1      &&
				   this.ThresholdValue0    == other.ThresholdValue0    &&
				   this.ThresholdValue1    == other.ThresholdValue1    &&
				   this.GraphPoints        == other.GraphPoints        &&
				   this.HasLines           == other.HasLines           &&
				   this.HasLegend          == other.HasLegend          &&
				   this.LegendPositionRel  == other.LegendPositionRel  &&
				   this.LegendColumns      == other.LegendColumns      &&
				   this.FontSize           == other.FontSize           &&
				   this.BorderThickness    == other.BorderThickness    &&
				   this.BarThickness       == other.BarThickness       &&
				   this.BarOverlap         == other.BarOverlap         &&
				   this.lineAlpha          == other.lineAlpha          &&
				   this.LineWidth          == other.LineWidth          &&
				   this.PointWidth         == other.PointWidth         &&
				   this.ExplodedPieFactor  == other.ExplodedPieFactor  &&
				   this.PiePercents        == other.PiePercents        &&
				   this.PieValues          == other.PieValues          &&
				   this.PrimaryDimension   == other.PrimaryDimension   &&
				   this.SecondaryDimension == other.SecondaryDimension &&
				   GraphOptions.CompareList (this.primaryFilter,   other.primaryFilter) &&
				   GraphOptions.CompareList (this.secondaryFilter, other.secondaryFilter);
		}

		private static bool CompareList(List<FormattedText> list1, List<FormattedText> list2)
		{
			if (list1.Count != list2.Count)
			{
				return false;
			}

			for (int i = 0; i < list1.Count; i++)
			{
				if (list1[i] != list2[i])
				{
					return false;
				}
			}

			return true;
		}


		public FormattedText Summary
		{
			get
			{
				return GraphOptions.GetDescription (this.mode);
			}
		}

		public static FormattedText GetDescription(GraphMode mode)
		{
			switch (mode)
			{
				case GraphMode.Stacked:
					return "Histogramme empilé";

				case GraphMode.SideBySide:
					return "Histogramme côte à côte";

				case GraphMode.Lines:
					return "Courbes";

				case GraphMode.Pie:
					return "Secteurs";

				case GraphMode.Array:
					return "Données brutes";

				default:
					return "?";
			}
		}


		private readonly List<FormattedText>	primaryFilter;
		private readonly List<FormattedText>	secondaryFilter;

		private GraphMode						mode;
		private GraphStyle						style;
		private Margins							marginsAbs;
		private FormattedText					titleText;
		private Point							titlePositionRel;
		private Size							titleSizeAbs;
		private bool							startAtZero;
		private bool							hasThreshold0;
		private bool							hasThreshold1;
		private decimal							thresholdValue0;
		private decimal							thresholdValue1;
		private GraphPoint						graphPoints;
		private bool							hasLines;
		private bool							hasLegend;
		private Point							legendPositionRel;
		private int								legendColumns;
		private double							fontSize;
		private double							borderThickness;
		private double							barThickness;
		private double							barOverlap;
		private double							lineAlpha;
		private double							lineWidth;
		private double							pointWidth;
		private double							explodedPieFactor;
		private bool							piePercents;
		private bool							pieValues;
		private int								primaryDimension;
		private int								secondaryDimension;
	}
}
