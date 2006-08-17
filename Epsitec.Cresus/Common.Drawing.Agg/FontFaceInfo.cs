//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public sealed class FontFaceInfo
	{
		internal FontFaceInfo(string name)
		{
			this.name  = name;
			this.fonts = new List<Font> ();
		}
		
		internal void Add(Font font)
		{
			System.Diagnostics.Debug.Assert (this.fonts.Contains (font) == false);
			System.Diagnostics.Debug.Assert (font.FaceName == this.name);
			System.Diagnostics.Debug.Assert (font.FaceInfo == null);

			this.fonts.Add (font);
			font.DefineFaceInfo (this);
		}


		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public string[]							StyleNames
		{
			get
			{
				List<string> styles = new List<string> ();

				foreach (Font font in this.fonts)
				{
					string name = font.StyleName;

					if (string.IsNullOrEmpty (name))
					{
						name = "Regular";
					}

					if (styles.Contains (name) == false)
					{
						styles.Add (name);
					}
				}

				styles.Sort ();
				
				return styles.ToArray ();
			}
		}

		public int								Count
		{
			get
			{
				return this.fonts.Count;
			}
		}

		public bool								IsLatin
		{
			get
			{
				if (this.fonts.Count > 0)
				{
					Font font = this.fonts[0];
					return font.OpenTypeFont.FontIdentity.IsSymbolFont ? false : true;
				}
				
				return false;
			}
		}


		public Font GetFont(bool bold, bool italic)
		{
			foreach (Font font in this.fonts)
			{
				bool isBoldFont   = font.IsStyleBold;
				bool isItalicFont = font.IsStyleItalic;
				
				if ((bold == isBoldFont) &&
					(italic == isItalicFont))
				{
					return font;
				}
			}
			
			Font syntheticFont = this.GetSyntheticFont (bold, italic);

			if (syntheticFont != null)
			{
				this.Add (syntheticFont);
			}
			
			return syntheticFont;
		}

		public Font[] GetFonts()
		{
			return this.fonts.ToArray ();
		}


		private Font GetSyntheticFont(bool bold, bool italic)
		{
			string style = null;

			if (bold)
			{
				style = italic ? "Bold Italic" : "Bold";
			}
			else if (italic)
			{
				style = "Italic";
			}
			else
			{
				return null;
			}

			return Font.GetFont (this.name, style);
		}

		private string name;
		private List<Font> fonts;
	}
}
