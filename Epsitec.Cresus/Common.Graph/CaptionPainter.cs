﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				
				int i = 0;

				foreach (var sample in this.samples)
				{
					var size = this.sampleSizeCache[i++];
					
					oy -= size.Height;

					sample.Render (port, new Rectangle (ox, oy, dx, size.Height), style, textOffset);
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
