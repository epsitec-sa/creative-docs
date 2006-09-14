//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe BaseEngine sert de classe de base pour tous les moteurs de
	/// layout (LineEngine, etc.)
	/// </summary>
	public abstract class BaseEngine : System.IDisposable
	{
		public BaseEngine()
		{
		}
		
		
		public Text.TextContext						TextContext
		{
			get
			{
				return this.context;
			}
		}
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		
		public virtual void Initialize(Text.TextContext context, string name)
		{
			this.context = context;
			this.name    = name;
		}
		
		
		public virtual Layout.Status Fit(Layout.Context context, ref Layout.BreakCollection result)
		{
			return Layout.Status.Ok;
		}
		
		public virtual Layout.Status Render(Layout.Context context, ITextRenderer renderer, int length)
		{
			return Layout.Status.Ok;
		}
		
		public virtual Layout.Status FillProfile(Layout.Context context, int length, StretchProfile profile)
		{
			return Layout.Status.Ok;
		}
		
		
		public int GetRunLength(ulong[] text, int start, int length)
		{
			//	Détermine combien de caractères utilisent exactement les mêmes
			//	propriétés de style & réglages dans le texte passé en entrée.
			
			ulong code = Internal.CharMarker.ExtractCoreAndSettings (text[start]);
			
			if (Unicode.Bits.GetCode (text[start]) == (int) Unicode.Code.ObjectReplacement)
			{
				return 1;
			}
			if (Unicode.Bits.GetSpecialCodeFlag (text[start]))
			{
				return 1;
			}
			
			for (int i = 1; i < length; i++)
			{
				if (Unicode.Bits.GetCode (text[start+i]) == (int) Unicode.Code.ObjectReplacement)
				{
					return i;
				}
				if (Unicode.Bits.GetSpecialCodeFlag (text[start+i]))
				{
					return i;
				}
				
				if (Internal.CharMarker.ExtractCoreAndSettings (text[start+i]) != code)
				{
					return i;
				}
			}
			
			return length;
		}
		
		public int GetNextFragmentLength(ulong[] text, int start, int length, int fragment_length, out double break_penalty)
		{
			//	Détermine la taille d'un fragment de texte (prochaine césure) à
			//	partir d'une longueur de départ.
			
			for (int i = fragment_length; i < length; i++)
			{
				Unicode.BreakInfo info = Unicode.Bits.GetBreakInfo (text[start+i]);
				
				if (info == Unicode.BreakInfo.HyphenatePoorChoice)
				{
					break_penalty = 10.0;
					return i+1;
				}
				else if (info == Unicode.BreakInfo.HyphenateGoodChoice)
				{
					break_penalty = 5.0;
					return i+1;
				}
				
				Debug.Assert.IsTrue ((info == Unicode.BreakInfo.No) || (info == Unicode.BreakInfo.NoAlpha) || (i+1 == length));
			}
			
			break_penalty = 0;
			
			return length;
		}
		
		
		public static bool ContainsSpecialGlyphs(ulong[] text, int offset, int length)
		{
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[offset+i];
				
				if (Unicode.Bits.GetSpecialCodeFlag (bits))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public static bool GenerateGlyphs(Layout.Context context, OpenType.Font font, ulong[] text, int offset, int length, out ushort[] glyphs, ref byte[] attributes)
		{
			ulong[] temp = new ulong[length];
			bool special = false;
			
			System.Buffer.BlockCopy (text, offset * 8, temp, 0, length * 8);
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = temp[i];
				int   code = Unicode.Bits.GetCode (bits);
				
				if (Unicode.Bits.GetSpecialCodeFlag (bits))
				{
					ushort        s_glyph;
					OpenType.Font s_font;
					
					context.TextContext.GetGlyphAndFontForSpecialCode (bits, out s_glyph, out s_font);
					
					temp[i] = (ulong) ((int) s_glyph | (int) Unicode.Bits.SpecialCodeFlag);
					
					if ((special == false) &&
						(s_font.FontIdentity.FullName != font.FontIdentity.FullName))
					{
						special = true;
					}
				}
				else if (analyzer.IsZeroWidth (code))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if (analyzer.IsControl (code))
				{
					if (context.ShowControlCharacters)
					{
						temp[i] &= ~ Unicode.Bits.CodeMask;
						temp[i] |= BaseEngine.MapToVisibleControlCharacter (code) & Unicode.Bits.CodeMask;
					}
					else
					{
						temp[i] &= ~ Unicode.Bits.FullCodeMask;
					}
				}
				else if ((code == (int) Unicode.Code.SoftHyphen) &&
					/**/ (i+1 < length))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if ((code == (int) Unicode.Code.MongolianTodoHyphen) &&
					/**/ (i != 0))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
			}
			
			font.GenerateGlyphs (temp, 0, length, out glyphs, ref attributes);
			
			return special;
		}
		
		public static bool GenerateGlyphs(Layout.Context context, OpenType.Font font, ulong[] text, int offset, int length, out ushort[] glyphs, ref short[] attributes)
		{
			ulong[] temp = new ulong[length];
			bool special = false;
			
			System.Buffer.BlockCopy (text, offset * 8, temp, 0, length * 8);
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = temp[i];
				int   code = Unicode.Bits.GetCode (bits);
				
				if (Unicode.Bits.GetSpecialCodeFlag (bits))
				{
					ushort        s_glyph;
					OpenType.Font s_font;
					
					context.TextContext.GetGlyphAndFontForSpecialCode (bits, out s_glyph, out s_font);
					
					temp[i] = (ulong) ((int) s_glyph | (int) Unicode.Bits.SpecialCodeFlag);
					
					if ((special == false) &&
						(s_font.FontIdentity.FullName != font.FontIdentity.FullName))
					{
						special = true;
					}
				}
				else if (analyzer.IsZeroWidth (code))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if (analyzer.IsControl (code))
				{
					if (context.ShowControlCharacters)
					{
						temp[i] &= ~ Unicode.Bits.CodeMask;
						temp[i] |= BaseEngine.MapToVisibleControlCharacter (code) & Unicode.Bits.CodeMask;
					}
					else
					{
						temp[i] &= ~ Unicode.Bits.FullCodeMask;
					}
				}
				else if ((code == (int) Unicode.Code.SoftHyphen) &&
					/**/ (i+1 < length))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if ((code == (int) Unicode.Code.MongolianTodoHyphen) &&
					/**/ (i != 0))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
			}
			
			font.GenerateGlyphs (temp, 0, length, out glyphs, ref attributes);
			
			return special;
		}
		
		public static ulong MapToVisibleControlCharacter(int code)
		{
			switch ((Unicode.Code)code)
			{
				case Unicode.Code.ParagraphSeparator:
					code = 0x00B6;						//	¶ -- pilcrow sign
					break;
				
				case Unicode.Code.LineSeparator:
					code = 0x000A;
					break;
				
				case Unicode.Code.EndOfText:
					code = 0x00A4;						//	¤ -- currency sign
					break;
				
				default:
					code = 0;
					break;
			}
			
			return (ulong) code;
		}
		
		public static bool GenerateGlyphsAndStretchClassAttributes(Text.TextContext context, OpenType.Font font, ulong[] text, int offset, int length, out ushort[] glyphs, out byte[] attributes)
		{
			ulong[] temp = new ulong[length];
			bool special = false;
			
			System.Buffer.BlockCopy (text, offset * 8, temp, 0, length * 8);
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			attributes = new byte[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = temp[i];
				int   code = Unicode.Bits.GetCode (bits);
				
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (code);
				
				if (Unicode.Bits.GetSpecialCodeFlag (bits))
				{
					ushort        s_glyph;
					OpenType.Font s_font;
					
					context.GetGlyphAndFontForSpecialCode (bits, out s_glyph, out s_font);
					
					temp[i] = (ulong) ((int) s_glyph | (int) Unicode.Bits.SpecialCodeFlag);
					
					if ((special == false) &&
						(s_font.FontIdentity.FullName != font.FontIdentity.FullName))
					{
						special = true;
					}
				}
				else if (analyzer.IsControl (code) || analyzer.IsZeroWidth (code))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if ((code == (int) Unicode.Code.SoftHyphen) &&
					/**/ (i+1 < length))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
				else if ((code == (int) Unicode.Code.MongolianTodoHyphen) &&
					/**/ (i != 0))
				{
					temp[i] &= ~ Unicode.Bits.FullCodeMask;
				}
			}
			
			font.GenerateGlyphs (temp, 0, length, out glyphs, ref attributes);
			
			return special;
		}
		
		
		public static double GetSpaceWidth(OpenType.Font font, int code)
		{
			double space_width  = font.SpaceWidth;
			double en_width     = font.EnWidth;
			double em_width     = font.EmWidth;
			double figure_width = font.FigureWidth;
			double period_width = font.PeriodWidth;
			
			return Unicode.BreakAnalyzer.GetSpaceWidth (code, space_width, en_width, em_width, figure_width, period_width);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.context = null;
			}
		}
		
		
		private Text.TextContext					context;
		private string							name;
	}
}
