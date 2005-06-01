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
		
		
		public Text.Context						TextContext
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
		
		
		public virtual void Initialise(Text.Context context, string name)
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
			
			ulong code = Internal.CharMarker.ExtractStyleAndSettings (text[start]);
			
			for (int i = 1; i < length; i++)
			{
				if (Internal.CharMarker.ExtractStyleAndSettings (text[start+i]) != code)
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
		
		
		public static ushort[] GenerateGlyphs (OpenType.Font font, ulong[] text, int offset, int length)
		{
			ushort[] glyphs;
			
			BaseEngine.GenerateGlyphs (font, text, offset, length, out glyphs, null);
			
			return glyphs;
		}
		
		public static void GenerateGlyphs(OpenType.Font font, ulong[] text, int offset, int length, out ushort[] glyphs, byte[] attributes)
		{
			ulong[] temp = new ulong[length];
			
			System.Buffer.BlockCopy (text, offset * 8, temp, 0, length * 8);
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			for (int i = 0; i < length; i++)
			{
				int code = Unicode.Bits.GetCode (temp[i]);
				
				if (analyzer.IsControl (code))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
				else if ((code == (int) Unicode.Code.SoftHyphen) &&
					/**/ (i+1 < length))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
				else if ((code == (int) Unicode.Code.MongolianTodoHyphen) &&
					/**/ (i != 0))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
			}
			
			font.GenerateGlyphs (temp, 0, length, out glyphs, attributes);
		}
		
		public static void GenerateGlyphsAndStretchClassAttributes(OpenType.Font font, ulong[] text, int offset, int length, out ushort[] glyphs, out byte[] attributes)
		{
			ulong[] temp = new ulong[length];
			
			System.Buffer.BlockCopy (text, offset * 8, temp, 0, length * 8);
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			attributes = new byte[length];
			
			for (int i = 0; i < length; i++)
			{
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (Unicode.Bits.GetCode (temp[i]));
				
				int code = Unicode.Bits.GetCode (temp[i]);
				
				if (analyzer.IsControl (code))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
				else if ((code == (int) Unicode.Code.SoftHyphen) &&
					/**/ (i+1 < length))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
				else if ((code == (int) Unicode.Code.MongolianTodoHyphen) &&
					/**/ (i != 0))
				{
					temp[i] &= ~ Unicode.Bits.CodeMask;
				}
			}
			
			font.GenerateGlyphs (temp, 0, length, out glyphs, attributes);
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
		
		
		private Text.Context					context;
		private string							name;
	}
}
