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
			this.startAtZero        = true;
			this.graphPoints        = GraphPoint.Circle;
			this.hasLines           = true;
			this.hasLegend          = true;
			this.hasThreshold       = false;
			this.thresholdValue     = 0.05m;  // 5%
			this.fontSize           = 10.0;
			this.borderThickness    = 1.0;
			this.barThickness       = 0.8;
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

		public bool HasThreshold
		{
			get
			{
				return this.hasThreshold;
			}
			set
			{
				this.hasThreshold = value;
			}
		}

		public decimal ThresholdValue
		{
			get
			{
				return this.thresholdValue;
			}
			set
			{
				this.thresholdValue = value;
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
			dst.StartAtZero        = this.StartAtZero;
			dst.HasThreshold       = this.HasThreshold;
			dst.ThresholdValue     = this.ThresholdValue;
			dst.GraphPoints        = this.GraphPoints;
			dst.HasLines           = this.HasLines;
			dst.HasLegend          = this.HasLegend;
			dst.FontSize           = this.FontSize;
			dst.BorderThickness    = this.BorderThickness;
			dst.BarThickness       = this.BarThickness;
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
				   this.StartAtZero        == other.StartAtZero        &&
				   this.HasThreshold       == other.HasThreshold       &&
				   this.ThresholdValue     == other.ThresholdValue     &&
				   this.GraphPoints        == other.GraphPoints        &&
				   this.HasLines           == other.HasLines           &&
				   this.HasLegend          == other.HasLegend          &&
				   this.FontSize           == other.FontSize           &&
				   this.BorderThickness    == other.BorderThickness    &&
				   this.BarThickness       == other.BarThickness       &&
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


		private readonly List<FormattedText>	primaryFilter;
		private readonly List<FormattedText>	secondaryFilter;

		private GraphMode						mode;
		private GraphStyle						style;
		private bool							startAtZero;
		private bool							hasThreshold;
		private decimal							thresholdValue;
		private GraphPoint						graphPoints;
		private bool							hasLines;
		private bool							hasLegend;
		private double							fontSize;
		private double							borderThickness;
		private double							barThickness;
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
