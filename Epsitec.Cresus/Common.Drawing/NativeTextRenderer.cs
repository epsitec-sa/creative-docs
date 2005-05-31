//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe NativeTextRenderer permet d'afficher rapidement du texte natif
	/// pour autant que le pixmap utilisé soit compatible.
	/// </summary>
	public sealed class NativeTextRenderer
	{
		private NativeTextRenderer()
		{
		}
		
		
		public static void Draw(Pixmap pixmap, OpenType.Font font, double size, ushort[] glyphs, double[] x, double[] y, Color color)
		{
			int length = glyphs.Length;
			
			if ((length == 0) ||
				(pixmap.IsOSBitmap == false))
			{
				return;
			}
			
			uint r = (uint) (color.R * 256 - 0.1);
			uint g = (uint) (color.G * 256 - 0.1);
			uint b = (uint) (color.B * 256 - 0.1);
			
			
			System.IntPtr font_handle = font.GetFontHandle (size); 
			uint          win32_color = (b << 16) | (g << 8) | (r);
			
			double box_y = pixmap.Size.Height;
			double hy    = font.GetAscender (size);
			double oy    = y[0];
			int    start = 0;
			
			for (int i = 0; i < length; i++)
			{
				if (y[i] != oy)
				{
					NativeTextRenderer.ExtendedTextOut (pixmap, font_handle, (int) (box_y - oy - hy), glyphs, start, i-start, x, win32_color);
					start = i;
					oy    = y[i];
				}
			}
			
			NativeTextRenderer.ExtendedTextOut (pixmap, font_handle, (int) (box_y - oy - hy), glyphs, start, length-start, x, win32_color);
		}
		
		
		private static void ExtendedTextOut(Pixmap pixmap, System.IntPtr font_handle, int oy, ushort[] glyphs, int offset, int length, double[] x, uint color)
		{
			int[]    dx_array = new int[length];
			ushort[] text     = new ushort[length];
			int      count    = 0;
			
			int ox = (int) x[offset];
			
			int last_x = ox;
			
			for (int i = 0; i < length; i++)
			{
				ushort glyph = glyphs[offset+i];
				
				if (glyph != 0xffff)
				{
					text[count] = glyph;
					
					if (i > 0)
					{
						int xx = (int) x[offset+i];
						
						dx_array[count-1] = xx - last_x;
						last_x = xx;
					}
					
					count++;
				}
			}
			
			if (count > 0)
			{
				dx_array[count-1] = 0;
				
				AntiGrain.Buffer.DrawGlyphs (pixmap.Handle, font_handle, ox, oy, text, dx_array, count, color);
			}
		}
	}
}
