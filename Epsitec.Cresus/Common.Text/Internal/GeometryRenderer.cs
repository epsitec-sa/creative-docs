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
		}
		
		public void RenderStartLine(Layout.Context context)
		{
		}
		
		public void Render(ITextFrame frame, OpenType.Font font, double size, Drawing.Color color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			System.Diagnostics.Debug.Assert (frame != null);
			System.Diagnostics.Debug.Assert (font != null);
			System.Diagnostics.Debug.Assert (mapping != null);
			
			if (glyphs.Length == 0)
			{
				return;
			}
			
			int[]    map_char;
			ushort[] map_glyphs;
			
			int glyph_index = 0;
			
			while (mapping.GetNextMapping (out map_char, out map_glyphs))
			{
				System.Diagnostics.Debug.Assert (map_glyphs.Length == 1);
				
				double ox = x[glyph_index];
				double dx = x[glyph_index+1] - ox;
				double oy = y[glyph_index];
				
				double ax = dx / map_char.Length;
				
				if (map_char.Length > 1)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Ligature with {0} characters, dx={1}", map_char.Length, dx));
				}
				
				for (int i = 0; i < map_char.Length; i++)
				{
					this.items.Add (new Element (frame, font, size, map_char[i], ox, oy));
					
					ox += ax;
				}
				
				glyph_index++;
			}
			
			this.items.Add (new Element (frame, font, size, 0, x[glyph_index], y[glyph_index-1]));
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
			public Element(ITextFrame frame, OpenType.Font font, double size, int code, double x, double y)
			{
				this.frame = frame;
				
				this.font = font;
				this.size = size;
				this.code = code;
				
				this.x = x;
				this.y = y;
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
			
			
			private ITextFrame					frame;
			private OpenType.Font				font;
			private double						size;
			private int							code;
			private double						x;
			private double						y;
		}
		
		
		
		private System.Collections.ArrayList	items;
	}
}
