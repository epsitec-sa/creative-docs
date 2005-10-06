//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public void Render(Layout.Context layout, OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool is_last_run)
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
			
			int[]    map_char;
			ushort[] map_glyphs;
			
			double ascender  = layout.LineAscender;
			double descender = layout.LineDescender;
			
			double yb = layout.LineBaseY;
			double y1 = System.Math.Min (layout.LineY1, yb + descender);
			double y2 = System.Math.Max (layout.LineY2, yb + ascender);
			
			int glyph_index = 0;
			
			while (mapping.GetNextMapping (out map_char, out map_glyphs))
			{
				System.Diagnostics.Debug.Assert ((map_glyphs.Length == 1) || (is_last_run));
				
				double ox = x[glyph_index];
				double oy = y[glyph_index];
				double dx = x[glyph_index+1] - ox;
				
				double ax = dx / map_char.Length;
				
				//	S'il y a plusieurs caractères pour un glyphe donné (ligature),
				//	on répartit la largeur de manière égale entre les caractères.
				
				for (int i = 0; i < map_char.Length; i++, ox += ax)
				{
					this.items.Add (new Element (frame, font, size, map_char[i], ox, oy, y1, y2));
				}
				
				glyph_index++;
			}
			
			if (is_last_run)
			{
				//	Pour la dernière ligne, on enregistre encore un élément pour
				//	représenter la marque de fin de paragraphe :
				
				double ox = x[glyph_index];
				double oy = y[glyph_index-1];
				
				this.items.Add (new Element (frame, font, size, 0, ox, oy, y1, y2));
			}
		}
		
		public void Render(Layout.Context layout, IGlyphRenderer glyph_renderer, string color, double x, double y, bool is_last_run)
		{
			ITextFrame frame = layout.Frame;
			
			double y1 = layout.LineY1;
			double y2 = layout.LineY2;
			
			this.items.Add (new Element (frame, null, 0, 0, x, y, y1, y2));
			
			if (is_last_run)
			{
				double ascender, descender, advance, x1, x2;
				
				glyph_renderer.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
				
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
			
			
			public double						X
			{
				get
				{
					return this.x;
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
	}
}
