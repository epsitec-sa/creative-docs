//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;


namespace Epsitec.Common.Graph
{
	/// <summary>
	/// The <c>ChartCaption</c> class represents a caption describing a chart element
	/// (there is typically one <c>ChartCaption</c> for every <see cref="ChartSeries"/>).
	/// </summary>
	public class ChartCaption
	{
		public ChartCaption()
		{
		}

		
		public string Label
		{
			get;
			set;
		}
		
		public System.Action<IPaintPort, Rectangle> SamplePainter
		{
			get;
			set;
		}

		public double SampleWidth
		{
			get;
			set;
		}


		public Size GetLabelSize(Styles.CaptionStyle style, double width)
		{
			double textWidth  = style.GetTextWidth (this.Label);
			double textHeight = style.GetTextLineHeight ();

			if (textWidth > width)
			{
				//	TODO: layout using multiple lines

				textWidth = width;
			}

			return new Size (textWidth, textHeight);
		}

		public double Render(IPaintPort port, Rectangle bounds, Styles.CaptionStyle style, double textOffsetX)
		{
			double height = style.GetTextLineHeight ();

			var ox = bounds.Left;
			var oy = bounds.Top - height;
			var dx = bounds.Width;
			var dy = bounds.Height;
			
			var offset = style.GetTextLineOffset ();
			var color  = style.FontColor;

			this.SamplePainter (port, new Rectangle (ox, oy, this.SampleWidth, dy));

			if (!color.IsEmpty)
			{
				port.Color = color;
			}

			port.PaintText (ox + textOffsetX + offset.X, oy + offset.Y, this.Label, style.Font, style.FontSize);

			return height;
		}
	}
}
