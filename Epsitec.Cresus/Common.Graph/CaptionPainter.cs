//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph
{
	public class CaptionPainter
	{
		public CaptionPainter()
		{
			this.samples = new List<Sample> ();

			this.Font = Font.GetFont ("Calibri", "Regular");
			this.FontSize = 12;
		}


		public int SampleCount
		{
			get
			{
				return this.samples.Count;
			}
		}

		public Font Font
		{
			get;
			set;
		}

		public double FontSize
		{
			get;
			set;
		}

		
		public void AddSample(string label, System.Action<IPaintPort, Rectangle> painter)
		{
			this.samples.Add (
				new Sample ()
				{
					Label = label,
					Painter = painter
				});
		}


		public void Render(IPaintPort port, Rectangle bounds)
		{
			int count = this.SampleCount;

			if (count > 0)
			{
				foreach (var sample in this.samples)
				{
					sample.Width = this.Font.GetTextAdvance (sample.Label) * this.FontSize;
				}

				double ht = System.Math.Ceiling (this.Font.LineHeight * this.FontSize * 1.2);
				double h1 = ht/10 - this.Font.Descender * this.FontSize;

				double dx = this.samples.Max (x => x.Width) + 2.5*ht;
				double dy = ht * count;
				
				double ox = bounds.Left + (bounds.Width-dx) / 2;
				double oy = bounds.Top  - (bounds.Height-dy) / 2;
				
				foreach (var sample in this.samples)
				{
					oy -= ht;
					sample.Painter (port, new Rectangle (ox, oy, 2*ht, ht));
					port.PaintText (ox + 2.5*ht, oy + h1, sample.Label, this.Font, this.FontSize);
				}
			}
		}
		

		class Sample
		{
			public string Label
			{
				get;
				set;
			}
			
			public System.Action<IPaintPort, Rectangle> Painter
			{
				get;
				set;
			}

			public double Width
			{
				get;
				set;
			}
		}

		private readonly List<Sample> samples;
	}
}
