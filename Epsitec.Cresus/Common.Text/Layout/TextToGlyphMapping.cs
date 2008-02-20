//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe TextToGlyphMapping permet de d�terminer les correspondances
	/// entre un fragment de texte Unicode et les glyphes correspondants.
	/// </summary>
	public class TextToGlyphMapping
	{
		public TextToGlyphMapping(ulong[] text, int offset, int length, ushort[] glyphs, short[] map)
		{
			System.Diagnostics.Debug.Assert (glyphs.Length < map.Length);
			
			this.text   = new ulong[length];
			this.glyphs = glyphs;
			this.map    = map;
			
			for (int i = 0; i < length; i++)
			{
				this.text[i] = text[offset+i];
			}
		}
		
		
		public void Reset()
		{
			this.text_index  = 0;
			this.glyph_index = 0;
		}
		
		public bool GetNextMapping(out int[] characters, out ushort[] glyphs)
		{
			//	Retourne le prochain paquet de caract�res avec le prochain paquet
			//	de glyphes. Pour les cas simples, les deux paquets ont exactement
			//	un �l�ment (1 caract�re -> 1 glyphe).
			
			//	Dans le cas d'une ligature, on aura plusieurs caract�res pour un
			//	seul glyphe ("ffi" --> glyphe #123).
			
			//	Dans le cas de caract�res composites, on aura plusieurs glyphes
			//	pour un seul caract�re ("�" --> glyphes de 'a' + '^').

			if ((this.text_index < this.text.Length) &&
				(this.glyph_index < this.glyphs.Length))
			{
				short map_0 = this.map[this.glyph_index];
				
				int glyph_count = 1;
				
				for (int i = 1; this.glyph_index + i < this.glyphs.Length; i++)
				{
					if (this.map[this.glyph_index + i] != map_0)
					{
						break;
					}
					
					glyph_count++;
				}
				
				if (glyph_count > 1)
				{
					//	Il y a plusieurs glyphes pour repr�senter un unique
					//	caract�re de texte :
					
					characters = new int[1];
					glyphs     = new ushort[glyph_count];
					
					for (int i = 0; i < glyph_count; i++)
					{
						glyphs[i] = this.glyphs[this.glyph_index+i];
					}
					
					characters[0] = Unicode.Bits.GetCode (this.text[this.text_index]);
					
					this.text_index  += 1;
					this.glyph_index += glyph_count;
					
					return true;
				}
				
				//	Peut-�tre y a-t-il plusieurs caract�res repr�sent�s par cet
				//	unique glyphe :
				
				int char_count = this.map[this.glyph_index+1] - this.map[this.glyph_index];
				
				characters = new int[char_count];
				glyphs     = new ushort[1];
				
				for (int i = 0; i < char_count; i++)
				{
					characters[i] = Unicode.Bits.GetCode (this.text[this.text_index+i]);
				}
				
				glyphs[0] = this.glyphs[this.glyph_index];
				
				this.text_index  += char_count;
				this.glyph_index += 1;
				
				return true;
			}
			
			characters = null;
			glyphs     = null;
			
			return false;
		}
		
		public bool GetNextMapping(out int[] characters, out ushort[] glyphs, out ulong[] text)
		{
			if ((this.text_index < this.text.Length) &&
				(this.glyph_index < this.glyphs.Length))
			{
				short map_0 = this.map[this.glyph_index];
				
				int glyph_count = 1;
				
				for (int i = 1; this.glyph_index + i < this.glyphs.Length; i++)
				{
					if (this.map[this.glyph_index + i] != map_0)
					{
						break;
					}
					
					glyph_count++;
				}
				
				if (glyph_count > 1)
				{
					//	Il y a plusieurs glyphes pour repr�senter un unique
					//	caract�re de texte :
					
					characters = new int[1];
					text       = new ulong[1];
					glyphs     = new ushort[glyph_count];
					
					for (int i = 0; i < glyph_count; i++)
					{
						glyphs[i] = this.glyphs[this.glyph_index+i];
					}
					
					characters[0] = Unicode.Bits.GetCode (this.text[this.text_index]);
					text[0]       = this.text[this.text_index];
					
					this.text_index  += 1;
					this.glyph_index += glyph_count;
					
					return true;
				}
				
				//	Peut-�tre y a-t-il plusieurs caract�res repr�sent�s par cet
				//	unique glyphe :
				
				int char_count = this.map[this.glyph_index+1] - this.map[this.glyph_index];
				
				characters = new int[char_count];
				text       = new ulong[char_count];
				glyphs     = new ushort[1];
				
				for (int i = 0; i < char_count; i++)
				{
					characters[i] = Unicode.Bits.GetCode (this.text[this.text_index+i]);
					text[i]       = this.text[this.text_index+i];
				}
				
				glyphs[0] = this.glyphs[this.glyph_index];
				
				this.text_index  += char_count;
				this.glyph_index += 1;
				
				return true;
			}
			
			characters = null;
			text       = null;
			glyphs     = null;
			
			return false;
		}
		
		
		
		private ulong[]							text;
		private ushort[]						glyphs;
		private short[]							map;
		private int								text_index;
		private int								glyph_index;
	}
}
