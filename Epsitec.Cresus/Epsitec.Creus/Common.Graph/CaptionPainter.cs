//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph
{
	public class CaptionPainter
	{
		public CaptionPainter()
		{
			this.samples = new List<ChartCaption> ();
			this.sampleSizeCache = new List<Size> ();
		}


		public ContainerLayoutMode LayoutMode
		{
			get;
			set;	//	TODO: implement layout mode
		}

		public int SampleCount
		{
			get
			{
				return this.samples.Count;
			}
		}

		public Styles.CaptionStyle Style
		{
			get
			{
				return this.style ?? CaptionPainter.defaultStyle;
			}
			set
			{
				this.style = value;
			}
		}

		public System.Action<CaptionPainter, int, Rectangle, IPaintPort> BackgroundPaintCallback
		{
			get;
			set;
		}

		public void Clear()
		{
			this.samples.Clear ();
			this.sampleSizeCache.Clear ();
			this.sampleWidth = 0;
			this.sampleSizeCacheAvailableSize = Size.Empty;
			this.captionLayoutSize = Size.Empty;
		}
		
		public void AddSample(string label, System.Action<IPaintPort, Rectangle> painter)
		{
			var sample =
				new ChartCaption ()
				{
					Label = label,
					SamplePainter = painter,
					SampleWidth = CaptionPainter.defaultSampleWidth
				};
			
			this.samples.Add (sample);

			this.sampleWidth = System.Math.Max (this.sampleWidth, sample.SampleWidth);
			this.sampleSizeCache.Clear ();
		}


		/// <summary>
		/// Gets the size of the complete caption layout. The size is cached from one
		/// call to the next.
		/// </summary>
		/// <param name="availableWidth">Available width for the layout.</param>
		/// <returns>The size of the complete caption layout.</returns>
		public Size GetCaptionLayoutSize(Size availableSize)
		{
			this.UpdateCaptionLayoutSize (availableSize);
			
			return this.captionLayoutSize;
		}

		/// <summary>
		/// Renders the captions to the specified port.
		/// </summary>
		/// <param name="port">The rendering port.</param>
		/// <param name="bounds">The bounds used for the layout.</param>
		public void Render(IPaintPort port, Rectangle bounds)
		{
			int count = this.SampleCount;
			var style = this.Style;

			if (count > 0)
			{
				this.UpdateCaptionLayoutSize (bounds.Size);

				double ox = bounds.Left;
				double oy = bounds.Top;
				double dx = bounds.Width;

				double textOffset = this.sampleWidth + CaptionPainter.defaultSpaceWidth;
				
				int index = 0;

				foreach (var item in this.GetSampleRectangles (bounds))
				{
					var sample = item.Key;
					var rect   = item.Value;

					if (this.BackgroundPaintCallback != null)
					{
						this.BackgroundPaintCallback (this, index++, rect, port);
					}
					
					sample.Render (port, rect, style, textOffset);
				}
			}
		}

		/// <summary>
		/// Detects the caption which contains the specified position.
		/// </summary>
		/// <param name="bounds">The bounds used for the layout.</param>
		/// <param name="position">The position.</param>
		/// <returns>The index of the matching caption or <c>-1</c> if the detection fails.</returns>
		public int DetectCaption(Rectangle bounds, Point position)
		{
			int index = 0;

			foreach (var item in this.GetSampleRectangles (bounds))
			{
				if (item.Value.Contains (position))
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		public Rectangle GetCaptionBounds(Rectangle bounds, int index)
		{
			foreach (var item in this.GetSampleRectangles (bounds))
			{
				if (index-- == 0)
				{
					return item.Value;
				}
			}

			return Rectangle.Empty;
		}

		public ChartCaption GetSampleCaption(int index)
		{
			if ((index >= 0) &&
				(index < this.samples.Count))
			{
				return this.samples[index];
			}
			else
			{
				return null;
			}
		}

		
		private IEnumerable<KeyValuePair<ChartCaption, Rectangle>> GetSampleRectangles(Rectangle bounds)
		{
			int count = this.SampleCount;

			if (count > 0)
			{
				this.UpdateCaptionLayoutSize (bounds.Size);

				double ox = bounds.Left;
				double oy = bounds.Top;
				double dx = bounds.Width;

				double textOffset = this.sampleWidth + CaptionPainter.defaultSpaceWidth;

				int i = 0;

				foreach (var sample in this.samples)
				{
					var size = this.sampleSizeCache[i];
					oy -= size.Height;
					var rect = new Rectangle (ox, oy, dx, size.Height);
					
					yield return new KeyValuePair<ChartCaption, Rectangle> (sample, rect);
				}
			}
		}

		/// <summary>
		/// Updates the size of the caption layout, based on the available space.
		/// </summary>
		/// <param name="availableSize">Available size for the layout.</param>
		private void UpdateCaptionLayoutSize(Size availableSize)
		{
			if ((this.sampleSizeCacheAvailableSize != availableSize) ||
				(this.sampleSizeCache.Count != this.samples.Count))
			{
				int count = this.SampleCount;
				var style = this.Style;

				this.captionLayoutSize = Size.Empty;
				this.sampleSizeCacheAvailableSize = availableSize;
				this.sampleSizeCache.Clear ();

				if (count > 0)
				{
					var height = style.GetTextLineHeight ();

					var sampleWidth = this.sampleWidth + CaptionPainter.defaultSpaceWidth;
					var labelRoom   = availableSize.Width - sampleWidth;

					double dx = 0;
					double dy = 0;

					foreach (var sample in this.samples)
					{
						var sampleSize = sample.GetLabelSize (style, labelRoom);

						dx  = System.Math.Max (dx, sampleWidth + sampleSize.Width);
						dy += sampleSize.Height;

						this.sampleSizeCache.Add (sampleSize);
					}

					this.captionLayoutSize = new Size (dx, dy);
				}
			}
		}


		private readonly List<ChartCaption> samples;
		private readonly List<Size>			sampleSizeCache;
		
		private Styles.CaptionStyle			style;
		private double						sampleWidth;
		private Size						sampleSizeCacheAvailableSize;
		private Size						captionLayoutSize;

		
		private static readonly Styles.CaptionStyle	defaultStyle		= new Styles.CaptionStyle ();
		private static readonly double				defaultSampleWidth	= CaptionPainter.defaultStyle.GetTextLineHeight () * 2;
		private static readonly double				defaultSpaceWidth   = CaptionPainter.defaultSampleWidth / 4;
	}
}
