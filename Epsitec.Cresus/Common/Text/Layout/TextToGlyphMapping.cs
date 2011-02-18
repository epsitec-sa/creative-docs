//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe TextToGlyphMapping permet de déterminer les correspondances
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
			this.textIndex  = 0;
			this.glyphIndex = 0;
		}
		
		public bool GetNextMapping(out int[] characters, out ushort[] glyphs)
		{
			//	Retourne le prochain paquet de caractères avec le prochain paquet
			//	de glyphes. Pour les cas simples, les deux paquets ont exactement
			//	un élément (1 caractère -> 1 glyphe).
			
			//	Dans le cas d'une ligature, on aura plusieurs caractères pour un
			//	seul glyphe ("ffi" --> glyphe #123).
			
			//	Dans le cas de caractères composites, on aura plusieurs glyphes
			//	pour un seul caractère ("â" --> glyphes de 'a' + '^').

			if ((this.textIndex < this.text.Length) &&
				(this.glyphIndex < this.glyphs.Length))
			{
				short map0 = this.map[this.glyphIndex];
				
				int glyphCount = 1;
				
				for (int i = 1; this.glyphIndex + i < this.glyphs.Length; i++)
				{
					if (this.map[this.glyphIndex + i] != map0)
					{
						break;
					}
					
					glyphCount++;
				}
				
				if (glyphCount > 1)
				{
					//	Il y a plusieurs glyphes pour représenter un unique
					//	caractère de texte :
					
					characters = new int[1];
					glyphs     = new ushort[glyphCount];
					
					for (int i = 0; i < glyphCount; i++)
					{
						glyphs[i] = this.glyphs[this.glyphIndex+i];
					}
					
					characters[0] = Unicode.Bits.GetCode (this.text[this.textIndex]);
					
					this.textIndex  += 1;
					this.glyphIndex += glyphCount;
					
					return true;
				}
				
				//	Peut-être y a-t-il plusieurs caractères représentés par cet
				//	unique glyphe :
				
				int charCount = this.map[this.glyphIndex+1] - this.map[this.glyphIndex];
				
				characters = new int[charCount];
				glyphs     = new ushort[1];
				
				for (int i = 0; i < charCount; i++)
				{
					characters[i] = Unicode.Bits.GetCode (this.text[this.textIndex+i]);
				}
				
				glyphs[0] = this.glyphs[this.glyphIndex];
				
				this.textIndex  += charCount;
				this.glyphIndex += 1;
				
				return true;
			}
			
			characters = null;
			glyphs     = null;
			
			return false;
		}
		
		public bool GetNextMapping(out int[] characters, out ushort[] glyphs, out ulong[] text)
		{
			if ((this.textIndex < this.text.Length) &&
				(this.glyphIndex < this.glyphs.Length))
			{
				short map0 = this.map[this.glyphIndex];
				
				int glyphCount = 1;
				
				for (int i = 1; this.glyphIndex + i < this.glyphs.Length; i++)
				{
					if (this.map[this.glyphIndex + i] != map0)
					{
						break;
					}
					
					glyphCount++;
				}
				
				if (glyphCount > 1)
				{
					//	Il y a plusieurs glyphes pour représenter un unique
					//	caractère de texte :
					
					characters = new int[1];
					text       = new ulong[1];
					glyphs     = new ushort[glyphCount];
					
					for (int i = 0; i < glyphCount; i++)
					{
						glyphs[i] = this.glyphs[this.glyphIndex+i];
					}
					
					characters[0] = Unicode.Bits.GetCode (this.text[this.textIndex]);
					text[0]       = this.text[this.textIndex];
					
					this.textIndex  += 1;
					this.glyphIndex += glyphCount;
					
					return true;
				}
				
				//	Peut-être y a-t-il plusieurs caractères représentés par cet
				//	unique glyphe :
				
				int charCount = this.map[this.glyphIndex+1] - this.map[this.glyphIndex];
				
				characters = new int[charCount];
				text       = new ulong[charCount];
				glyphs     = new ushort[1];
				
				for (int i = 0; i < charCount; i++)
				{
					characters[i] = Unicode.Bits.GetCode (this.text[this.textIndex+i]);
					text[i]       = this.text[this.textIndex+i];
				}
				
				glyphs[0] = this.glyphs[this.glyphIndex];
				
				this.textIndex  += charCount;
				this.glyphIndex += 1;
				
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
		private int								textIndex;
		private int								glyphIndex;
	}
}
