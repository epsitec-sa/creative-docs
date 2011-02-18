//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe GeometryRenderer permet de générer l'information de géométrie
	/// d'une ligne.
	/// </summary>
	internal class GeometryRenderer : ITextRenderer
	{
		public GeometryRenderer()
		{
			this.items = new System.Collections.ArrayList ();
		}
		
		
		public GeometryRenderer.Element			this[int index]
		{
			get
			{
				if ((index >= 0) &&
					(index < this.items.Count))
				{
					return this.items[index] as GeometryRenderer.Element;
				}
				else
				{
					return null;
				}
			}
		}
		
		public int								ElementCount
		{
			get
			{
				return this.items.Count;
			}
		}
		
		
		public bool								HasTabBeforeText
		{
			get
			{
				return double.IsNaN (this.tabOrigin) == false;
			}
		}
		
		public double							TabOrigin
		{
			get
			{
				return this.tabOrigin;
			}
		}
		
		public double							TabStop
		{
			get
			{
				return this.tabStop;
			}
		}
		
		
		public void DefineTab(double tabOrigin, double tabStop)
		{
			System.Diagnostics.Debug.Assert (this.ElementCount > 1);
			
			Element tabElement = this[this.ElementCount-2];
			Element endElement = this[this.ElementCount-1];
			
			endElement.X = tabStop;
		}
		
		
		#region ITextRenderer Members
		public bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height)
		{
			return true;
		}
		
		public void RenderStartParagraph(Layout.Context context)
		{
			System.Diagnostics.Debug.Assert (context.IsSimpleRenderingDisabled);
			System.Diagnostics.Debug.Assert (context.IsFontBaselineOffsetDisabled);
		}
		
		public void RenderStartLine(Layout.Context context)
		{
		}
		
		public void RenderTab(Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto)
		{
			this.tabOrigin = tabOrigin;
			this.tabStop   = tabStop;
		}
		
		public void Render(Layout.Context layout, OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
		{
			//	Enregistre la position des divers caractères qui composent le texte
			//	avec la fonte courante. Peuple la liste des éléments à cet effet.
			
			if (glyphs.Length == 0)
			{
				return;
			}
			
			ITextFrame frame = layout.Frame;
			
			System.Diagnostics.Debug.Assert (frame != null);
			System.Diagnostics.Debug.Assert (font != null);
			System.Diagnostics.Debug.Assert (mapping != null);
			
			int[]    mapChar;
			ushort[] mapGlyphs;
			
			double ascender  = layout.LineAscender;
			double descender = layout.LineDescender;
			
			double yb = layout.LineBaseY;
			double y1 = System.Math.Min (layout.LineY1, yb + descender);
			double y2 = System.Math.Max (layout.LineY2, yb + ascender);
			
			int glyphIndex = 0;
			
			while (mapping.GetNextMapping (out mapChar, out mapGlyphs))
			{
				System.Diagnostics.Debug.Assert ((mapGlyphs.Length == 1) || (isLastRun));
				
				double ox = x[glyphIndex];
				double oy = y[glyphIndex];
				double dx = x[glyphIndex+1] - ox;
				
				double ax = dx / mapChar.Length;
				
				//	S'il y a plusieurs caractères pour un glyphe donné (ligature),
				//	on répartit la largeur de manière égale entre les caractères.
				
				for (int i = 0; i < mapChar.Length; i++, ox += ax)
				{
					this.items.Add (new Element (frame, font, size, mapChar[i], ox, oy, y1, y2));
				}
				
				glyphIndex++;
			}
			
			if (isLastRun)
			{
				//	Pour la dernière ligne, on enregistre encore un élément pour
				//	représenter la marque de fin de paragraphe :
				
				double ox = x[glyphIndex];
				double oy = y[glyphIndex-1];
				
				this.items.Add (new Element (frame, font, size, 0, ox, oy, y1, y2));
			}
		}
		
		public void Render(Layout.Context layout, IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
		{
			ITextFrame frame = layout.Frame;
			
			double y1 = layout.LineY1;
			double y2 = layout.LineY2;
			
			this.items.Add (new Element (frame, null, 0, 0, x, y, y1, y2));
			
			if (isLastRun)
			{
				double ascender, descender, advance, x1, x2;
				
				glyphRenderer.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
				
				this.items.Add (new Element (frame, null, 0, 0, x + advance, y, y1, y2));
			}
		}
		
		public void RenderEndLine(Layout.Context context)
		{
		}
		
		public void RenderEndParagraph(Layout.Context context)
		{
		}
		#endregion
		
		public class Element
		{
			public Element(ITextFrame frame, OpenType.Font font, double size, int code, double x, double y, double y1, double y2)
			{
				this.frame = frame;
				
				this.font = font;
				this.size = size;
				this.code = code;
				
				this.x = x;
				this.y = y;
				
				this.y1 = y1;
				this.y2 = y2;
			}
			
			
			public Unicode.Code					Unicode
			{
				get
				{
					return (Unicode.Code) this.code;
				}
			}
			
			
			public ITextFrame					Frame
			{
				get
				{
					return this.frame;
				}
			}
			
			public OpenType.Font				Font
			{
				get
				{
					return this.font;
				}
			}
			
			public double						Size
			{
				get
				{
					return this.size;
				}
			}
			
			
			public double						X
			{
				get
				{
					return this.x;
				}
				set
				{
					this.x = value;
				}
			}
			
			public double						Y
			{
				get
				{
					return this.y;
				}
			}
			
			public double						Y1
			{
				get
				{
					return this.y1;
				}
			}
			
			public double						Y2
			{
				get
				{
					return this.y2;
				}
			}
			
			
			private ITextFrame					frame;
			private OpenType.Font				font;
			private double						size;
			private int							code;
			private double						x;
			private double						y, y1, y2;
		}
		
		
		
		private System.Collections.ArrayList	items;
		private double							tabOrigin = double.NaN;
		private double							tabStop = double.NaN;
	}
}
