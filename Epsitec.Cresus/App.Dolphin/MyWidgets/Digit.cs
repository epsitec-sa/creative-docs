//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un affichage à 7 segments et un point décimal pour afficher un digit.
	/// Il est possible de représenter une valeur hexadécimale (propriété HexValue)
	/// ou d'allumer n'importes quels segments (propriété SegmentValue).
	/// Les proportions idéales sont une hauteur 1.5x plus grande que la largeur.
	/// </summary>
	public class Digit : Widget
	{
		[System.Flags] public enum DigitSegment
		{
			TopRight    = 0x01,  // segment vertical
			BottomRight = 0x02,  // segment vertical
			Bottom      = 0x04,  // segment horizontal
			BottomLeft  = 0x08,  // segment vertical
			TopLeft     = 0x10,  // segment vertical
			Top         = 0x20,  // segment horizontal
			Middle      = 0x40,  // segment horizontal
			Dot         = 0x80,  // point décimal en bas à droite
		}


		public Digit() : base()
		{
		}

		public Digit(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public DigitSegment SegmentValue
		{
			//	Segments allumés.
			get
			{
				return this.segmentValue;
			}
			set
			{
				if (this.segmentValue != value)
				{
					this.segmentValue = value;
					this.Invalidate();
				}
			}
		}

		public int HexValue
		{
			//	Valeur hexadécimale [0..15] représentée.
			//	Retourne -1 si les segments correspondent à autre chose.
			get
			{
				return Digit.SegmentToHex(this.SegmentValue);
			}
			set
			{
				this.SegmentValue = Digit.HexToSegment(value);
			}
		}


		protected static DigitSegment HexToSegment(int value)
		{
			//	Conversion d'une valeur [0..15] dans les segments correspondants.
			if (value >= 0 && value < Digit.HexTable.Length)
			{
				return Digit.HexTable[value];
			}
			else
			{
				return DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom;
			}
		}

		protected static int SegmentToHex(DigitSegment segment)
		{
			//	Retourne la valeur [0..15] correspondant aux segments allumés.
			//	Retourne -1 si les segments correspondent à autre chose.
			for (int i=0; i<Digit.HexTable.Length; i++)
			{
				if (segment == Digit.HexTable[i])
				{
					return i;
				}
			}
			return -1;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(DolphinApplication.FromBrightness(0));

			for (int i=0; i<Digit.EnumTable.Length; i++)  // TODO: on peut faire mieux, mais je ne trouve plus...
			{
				DigitSegment segment = Digit.EnumTable[i];

				Path path = this.GetSegmentPath(segment);
				System.Diagnostics.Debug.Assert(path != null);

				Color color = ((this.segmentValue & segment) != 0) ? DolphinApplication.ColorHilite : Color.FromBrightness(0.2);

				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(color);  // dessine le segment
				path.Dispose();
			}
		}

		protected Path GetSegmentPath(DigitSegment oneSegment)
		{
			//	Retourne le chemin pour dessiner un segment.
			Path path = null;

			switch (oneSegment)
			{
				case DigitSegment.TopLeft:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.30, 0.87));
					path.LineTo(this.GetSegmentPoint(0.21, 0.52));
					path.LineTo(this.GetSegmentPoint(0.31, 0.52));
					path.LineTo(this.GetSegmentPoint(0.40, 0.87));
					path.Close();
					break;

				case DigitSegment.BottomLeft:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.19, 0.47));
					path.LineTo(this.GetSegmentPoint(0.10, 0.13));
					path.LineTo(this.GetSegmentPoint(0.20, 0.13));
					path.LineTo(this.GetSegmentPoint(0.29, 0.47));
					path.Close();
					break;

				case DigitSegment.TopRight:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.80, 0.87));
					path.LineTo(this.GetSegmentPoint(0.71, 0.52));
					path.LineTo(this.GetSegmentPoint(0.81, 0.52));
					path.LineTo(this.GetSegmentPoint(0.90, 0.87));
					path.Close();
					break;

				case DigitSegment.BottomRight:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.69, 0.47));
					path.LineTo(this.GetSegmentPoint(0.60, 0.13));
					path.LineTo(this.GetSegmentPoint(0.70, 0.13));
					path.LineTo(this.GetSegmentPoint(0.79, 0.47));
					path.Close();
					break;

				case DigitSegment.Top:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.48, 0.90));
					path.LineTo(this.GetSegmentPoint(0.45, 0.80));
					path.LineTo(this.GetSegmentPoint(0.72, 0.80));
					path.LineTo(this.GetSegmentPoint(0.75, 0.90));
					path.Close();
					break;

				case DigitSegment.Middle:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.38, 0.55));
					path.LineTo(this.GetSegmentPoint(0.35, 0.45));
					path.LineTo(this.GetSegmentPoint(0.62, 0.45));
					path.LineTo(this.GetSegmentPoint(0.65, 0.55));
					path.Close();
					break;

				case DigitSegment.Bottom:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.28, 0.20));
					path.LineTo(this.GetSegmentPoint(0.25, 0.10));
					path.LineTo(this.GetSegmentPoint(0.52, 0.10));
					path.LineTo(this.GetSegmentPoint(0.55, 0.20));
					path.Close();
					break;

				case DigitSegment.Dot:
					path = new Path();
					path.MoveTo(this.GetSegmentPoint(0.80, 0.20));
					path.LineTo(this.GetSegmentPoint(0.80, 0.10));
					path.LineTo(this.GetSegmentPoint(0.93, 0.10));
					path.LineTo(this.GetSegmentPoint(0.93, 0.20));
					path.Close();
					break;
			}

			return path;
		}

		protected Point GetSegmentPoint(double x, double y)
		{
			//	Retourne les coordonnées d'un point d'un segment.
			Rectangle rect = this.Client.Bounds;
			return new Point(rect.Left+rect.Width*x, rect.Bottom+rect.Height*y);
		}


		protected static DigitSegment[] HexTable =
		{
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Bottom,  // 0
			DigitSegment.TopRight | DigitSegment.BottomRight,  // 1
			DigitSegment.BottomLeft | DigitSegment.TopRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 2
			DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 3
			DigitSegment.TopLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Middle,  // 4
			DigitSegment.TopLeft | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 5
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 6
			DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top,  // 7
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 8
			DigitSegment.TopLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // 9
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Top | DigitSegment.Middle,  // A
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.BottomRight | DigitSegment.Middle | DigitSegment.Bottom,  // B
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.Top | DigitSegment.Bottom,  // C
			DigitSegment.BottomLeft | DigitSegment.TopRight | DigitSegment.BottomRight | DigitSegment.Middle | DigitSegment.Bottom,  // D
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.Top | DigitSegment.Middle | DigitSegment.Bottom,  // E
			DigitSegment.TopLeft | DigitSegment.BottomLeft | DigitSegment.Top | DigitSegment.Middle,  // F
		};

		protected static DigitSegment[] EnumTable =
		{
			DigitSegment.TopRight,
			DigitSegment.BottomRight,
			DigitSegment.Bottom,
			DigitSegment.BottomLeft,
			DigitSegment.TopLeft,
			DigitSegment.Top,
			DigitSegment.Middle,
			DigitSegment.Dot,
		};

		
		protected DigitSegment segmentValue;
	}
}
